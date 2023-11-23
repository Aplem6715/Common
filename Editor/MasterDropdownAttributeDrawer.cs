using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace Aplem.Common
{
    [CustomPropertyDrawer(typeof(MasterDropdownAttribute))]
    public class MasterDropdownAttributeDrawer : OdinAttributeDrawer<MasterDropdownAttribute>
    {
        private ValueDropdownList<int> Init()
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", Attribute.MasterType.Name),
                new string[] { Attribute.Folder });
            if (guids.Length == 0)
                return null;

            var guid = guids[0];
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var tableSO = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            var field = Attribute.MasterType.BaseType.GetField("_masterTable",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(tableSO);
            var masterList = (IList)value;

            var ret = new ValueDropdownList<int>();
            for (var i = 0; i < masterList.Count; i++)
            {
                var master = (IMasterData)masterList[i];
                var id = master.GetId();
                ret.Add($"{id}: {master.Label}", id);
            }

            return ret;
        }


        /**  from Odin/ValueDropdownAttributeDrawer.cs  **/

        // rawGetterをオリジナルのInit関数に置き換える実装
        // ValueDropdownAttributeDrawerがsealedだったので継承せずにコピー

        // private string error;
        private GUIContent label;

        private bool isList;

        private bool isListElement;

        private Func<IEnumerable<ValueDropdownItem>> getValues;

        private Func<IEnumerable<object>> getSelection;

        private IEnumerable<object> result;

        private bool enableMultiSelect;

        private Dictionary<object, string> nameLookup;

        // private ValueResolver<object> rawGetter;

        private LocalPersistentContext<bool> isToggled;

        //
        // 概要:
        //     Initializes this instance.
        protected override void Initialize()
        {
            isToggled = this.GetPersistentValue("Toggled", SirenixEditorGUI.ExpandFoldoutByDefault);
            isList = Property.ChildResolver is ICollectionResolver;
            isListElement = Property.Parent != null && Property.Parent.ChildResolver is ICollectionResolver;
            getSelection = () => Property.ValueEntry.WeakValues.Cast<object>();
            getValues = delegate
            {
                object value = Init();
                return value != null
                    ? (from object x in value as IEnumerable
                        where x != null
                        select x).Select(delegate(object x)
                    {
                        if (x is ValueDropdownItem)
                            return (ValueDropdownItem)x;

                        if (x is IValueDropdownItem)
                        {
                            var valueDropdownItem = x as IValueDropdownItem;
                            return new ValueDropdownItem(valueDropdownItem.GetText(), valueDropdownItem.GetValue());
                        }

                        return new ValueDropdownItem(null, x);
                    })
                    : null;
            };
            ReloadDropdownCollections();
        }

        private void ReloadDropdownCollections()
        {
            // if (error != null)
            // {
            //     return;
            // }

            object obj = null;
            object value = Init();
            if (value != null)
                obj = (value as IEnumerable).Cast<object>().FirstOrDefault();

            if (obj is IValueDropdownItem)
            {
                var enumerable = getValues();
                nameLookup = new Dictionary<object, string>(new IValueDropdownEqualityComparer(false));
                {
                    foreach (var item in enumerable)
                        nameLookup[item] = item.Text;

                    return;
                }
            }

            nameLookup = null;
        }

        private static IEnumerable<ValueDropdownItem> ToValueDropdowns(IEnumerable<object> query)
        {
            return query.Select(delegate(object x)
            {
                if (x is ValueDropdownItem)
                    return (ValueDropdownItem)x;

                if (x is IValueDropdownItem)
                {
                    var valueDropdownItem = x as IValueDropdownItem;
                    return new ValueDropdownItem(valueDropdownItem.GetText(), valueDropdownItem.GetValue());
                }

                return new ValueDropdownItem(null, x);
            });
        }

        //
        // 概要:
        //     Draws the property with GUILayout support. This method is called by DrawPropertyImplementation
        //     if the GUICallType is set to GUILayout, which is the default.
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.label = label;
            if (Property.ValueEntry == null)
            {
                CallNextDrawer(label);
            }
            // else if (error != null)
            // {
            //     SirenixEditorGUI.ErrorMessageBox(error);
            //     CallNextDrawer(label);
            // }
            else if (isList)
            {
                if (Attribute.DisableListAddButtonBehaviour)
                {
                    CallNextDrawer(label);
                    return;
                }

                var nextCustomAddFunction = CollectionDrawerStaticInfo.NextCustomAddFunction;
                CollectionDrawerStaticInfo.NextCustomAddFunction = OpenSelector;
                CallNextDrawer(label);
                if (result != null)
                {
                    AddResult(result);
                    result = null;
                }

                CollectionDrawerStaticInfo.NextCustomAddFunction = nextCustomAddFunction;
            }
            else if (Attribute.DrawDropdownForListElements || !isListElement)
            {
                DrawDropdown();
            }
            else
            {
                CallNextDrawer(label);
            }
        }

        private void AddResult(IEnumerable<object> query)
        {
            if (isList)
            {
                var collectionResolver = Property.ChildResolver as ICollectionResolver;
                if (enableMultiSelect)
                    collectionResolver.QueueClear();

                {
                    foreach (var item in query)
                    {
                        var array = new object[Property.ParentValues.Count];
                        for (var i = 0; i < array.Length; i++)
                            if (Attribute.CopyValues)
                                array[i] = Sirenix.Serialization.SerializationUtility.CreateCopy(item);
                            else
                                array[i] = item;

                        collectionResolver.QueueAdd(array);
                    }

                    return;
                }
            }

            var obj = query.FirstOrDefault();
            for (var j = 0; j < Property.ValueEntry.WeakValues.Count; j++)
                if (Attribute.CopyValues)
                    Property.ValueEntry.WeakValues[j] = Sirenix.Serialization.SerializationUtility.CreateCopy(obj);
                else
                    Property.ValueEntry.WeakValues[j] = obj;
        }

        private void DrawDropdown()
        {
            IEnumerable<object> enumerable = null;
            if (Attribute.AppendNextDrawer && !isList)
            {
                GUILayout.BeginHorizontal();
                var num = 15f;
                if (label != null)
                    num += GUIHelper.BetterLabelWidth;

                var gUIContent = GUIHelper.TempContent("");
                if (Property.Info.TypeOfValue == typeof(Type))
                    gUIContent.image =
                        GUIHelper.GetAssetThumbnail(null, Property.ValueEntry.WeakSmartValue as Type, false);

                enumerable = OdinSelector<object>.DrawSelectorDropdown(label, gUIContent, ShowSelector,
                    !Attribute.OnlyChangeValueOnConfirm, GUIStyle.none, GUILayoutOptions.Width(num));
                if (Event.current.type == EventType.Repaint)
                {
                    var position = GUILayoutUtility.GetLastRect().AlignRight(15f);
                    position.y += 4f;
                    SirenixGUIStyles.PaneOptions.Draw(position, GUIContent.none, 0);
                }

                GUILayout.BeginVertical();
                var disableGUIInAppendedDrawer = Attribute.DisableGUIInAppendedDrawer;
                if (disableGUIInAppendedDrawer)
                    GUIHelper.PushGUIEnabled(false);

                CallNextDrawer(null);
                if (disableGUIInAppendedDrawer)
                    GUIHelper.PopGUIEnabled();

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            else
            {
                var gUIContent2 = GUIHelper.TempContent(GetCurrentValueName());
                if (Property.Info.TypeOfValue == typeof(Type))
                    gUIContent2.image =
                        GUIHelper.GetAssetThumbnail(null, Property.ValueEntry.WeakSmartValue as Type, false);

                if (!Attribute.HideChildProperties && Property.Children.Count > 0)
                {
                    isToggled.Value = SirenixEditorGUI.Foldout(isToggled.Value, label, out var valueRect);
                    enumerable = OdinSelector<object>.DrawSelectorDropdown(valueRect, gUIContent2, ShowSelector,
                        !Attribute.OnlyChangeValueOnConfirm);
                    if (SirenixEditorGUI.BeginFadeGroup(this, isToggled.Value))
                    {
                        EditorGUI.indentLevel++;
                        for (var i = 0; i < Property.Children.Count; i++)
                        {
                            var inspectorProperty = Property.Children[i];
                            inspectorProperty.Draw(inspectorProperty.Label);
                        }

                        EditorGUI.indentLevel--;
                    }

                    SirenixEditorGUI.EndFadeGroup();
                }
                else
                {
                    enumerable = OdinSelector<object>.DrawSelectorDropdown(label, gUIContent2, ShowSelector,
                        !Attribute.OnlyChangeValueOnConfirm, null);
                }
            }

            if (enumerable != null && enumerable.Any())
                AddResult(enumerable);
        }

        private void OpenSelector()
        {
            ReloadDropdownCollections();
            var rect = new Rect(Event.current.mousePosition, Vector2.zero);
            var odinSelector = ShowSelector(rect);
            odinSelector.SelectionConfirmed += delegate(IEnumerable<object> x) { result = x; };
        }

        private OdinSelector<object> ShowSelector(Rect rect)
        {
            var genericSelector = CreateSelector();
            rect.x = (int)rect.x;
            rect.y = (int)rect.y;
            rect.width = (int)rect.width;
            rect.height = (int)rect.height;
            if (Attribute.AppendNextDrawer && !isList)
                rect.xMax = GUIHelper.GetCurrentLayoutRect().xMax;

            genericSelector.ShowInPopup(rect, new Vector2(Attribute.DropdownWidth, Attribute.DropdownHeight));
            return genericSelector;
        }

        private GenericSelector<object> CreateSelector()
        {
            var isUniqueList = Attribute.IsUniqueList;
            var enumerable = getValues() ?? Enumerable.Empty<ValueDropdownItem>();
            if (enumerable.Any())
            {
                if ((isList && Attribute.ExcludeExistingValuesInList) || (isListElement && isUniqueList))
                {
                    var list = enumerable.ToList();
                    var inspectorProperty =
                        Property.FindParent((InspectorProperty x) => x.ChildResolver is ICollectionResolver, true);
                    var comparer = new IValueDropdownEqualityComparer(false);
                    inspectorProperty.ValueEntry.WeakValues.Cast<IEnumerable>()
                        .SelectMany((IEnumerable x) => x.Cast<object>()).ForEach(delegate(object x)
                        {
                            list.RemoveAll((ValueDropdownItem c) => comparer.Equals(c, x));
                        });
                    enumerable = list;
                }

                if (nameLookup != null)
                    foreach (var item in enumerable)
                        if (item.Value != null)
                            nameLookup[item.Value] = item.Text;
            }

            var drawSearchToolbar = Attribute.NumberOfItemsBeforeEnablingSearch == 0 || (enumerable != null &&
                enumerable.Take(Attribute.NumberOfItemsBeforeEnablingSearch).Count() ==
                Attribute.NumberOfItemsBeforeEnablingSearch);
            var genericSelector = new GenericSelector<object>(Attribute.DropdownTitle, false,
                enumerable.Select((ValueDropdownItem x) => new GenericSelectorItem<object>(x.Text, x.Value)));
            enableMultiSelect = isList && isUniqueList && !Attribute.ExcludeExistingValuesInList;
            if (Attribute.FlattenTreeView)
                genericSelector.FlattenedTree = true;

            if (isList && !Attribute.ExcludeExistingValuesInList && isUniqueList)
                genericSelector.CheckboxToggle = true;
            else if (!Attribute.DoubleClickToConfirm && !enableMultiSelect)
                genericSelector.EnableSingleClickToSelect();

            if (isList && enableMultiSelect)
            {
                genericSelector.SelectionTree.Selection.SupportsMultiSelect = true;
                genericSelector.DrawConfirmSelectionButton = true;
            }

            genericSelector.SelectionTree.Config.DrawSearchToolbar = drawSearchToolbar;
            var selection = Enumerable.Empty<object>();
            if (!isList)
                selection = getSelection();
            else if (enableMultiSelect)
                selection = getSelection().SelectMany((object x) => (x as IEnumerable).Cast<object>());

            genericSelector.SetSelection(selection);
            genericSelector.SelectionTree.EnumerateTree().AddThumbnailIcons(true);
            if (Attribute.ExpandAllMenuItems)
                genericSelector.SelectionTree.EnumerateTree(delegate(OdinMenuItem x) { x.Toggled = true; });

            if (Attribute.SortDropdownItems)
                genericSelector.SelectionTree.SortMenuItemsByName();

            return genericSelector;
        }

        private string GetCurrentValueName()
        {
            if (!EditorGUI.showMixedValue)
            {
                var weakSmartValue = Property.ValueEntry.WeakSmartValue;
                string value = null;
                if (nameLookup != null && weakSmartValue != null)
                    nameLookup.TryGetValue(weakSmartValue, out value);

                return new GenericSelectorItem<object>(value, weakSmartValue).GetNiceName();
            }

            return "—";
        }
    }
}
