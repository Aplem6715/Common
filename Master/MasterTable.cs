using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using System.IO;
using System.Text;
using System.Linq;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aplem.Common
{
    public interface IMasterData
    {
        public int GetId();
        public string Label { get; }
        public string Name { get; }
    }

    public abstract class MasterTable<T, MasterT> : SingletonScriptableObject<T> where MasterT : IMasterData
    {
        public int RecordCount => _masterTable.Count;

        public IReadOnlyList<MasterT> DataList => _masterTable;

        [SerializeField, TableList]
        private List<MasterT> _masterTable = new List<MasterT>();

        private Dictionary<int, MasterT> _idMap;

#if UNITY_EDITOR
        private string _dataName = null;
        private static string _enumSourceDir => $"Assets/Project/Scripts/Data/Label";
#endif

        public override void OnAfterDeserialize()
        {
            _idMap = new Dictionary<int, MasterT>();
            foreach (MasterT master in _masterTable)
            {
                _idMap[master.GetId()] = master;
            }
        }

        public MasterT GetMaster(int id)
        {
            if (_idMap == null)
            {
                _logger.ZLogError("テーブルがロードされていません");
            }
            if (_idMap.TryGetValue(id, out MasterT master))
            {
                return master;
            }
            _logger.ZLogError("id:{} のレコードが見つかりません");
            return default;
        }

        public int GetRandomId(ref Unity.Mathematics.Random rand)
        {
            return _idMap.ElementAt(rand.NextInt(_idMap.Count)).Key;
        }

        protected override void Reset()
        {
            _idMap = null;
            _masterTable = null;
            // TODO: アセットアンロード
        }

#if UNITY_EDITOR
        private StringBuilder _builder = new StringBuilder();

        [PropertyOrder(-1), Button("      Generate Enum      ", ButtonSizes.Large, ButtonAlignment = 0.5f, Stretch = false)]
        private void GenerateEnum()
        {
            _dataName = typeof(T).Name.Replace("Table", "");
            if (!Directory.Exists(_enumSourceDir))
            {
                _logger.ZLogError("ディレクトリが見つかりません\n[{0}]", _enumSourceDir);
                return;
            }
            string filePath = $"{_enumSourceDir}/{_dataName}Label.cs";
            StreamWriter writer = File.CreateText(filePath);

            _builder.Clear();
            foreach (var row in _masterTable)
            {
                _builder.Append($"\n        /// <summary> {row.Name} </summary>");
                _builder.Append($"\n        {row.Label} = {row.GetId()},");
            }

            writer.Write(
$@"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     implementation: MasterTable.cs
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aplem.Project
{{
    public enum {_dataName}Label
    {{
        None,{_builder}
    }}
}}
");
            writer.Close();

            AssetDatabase.ImportAsset(filePath);
        }
#endif
    }
}
