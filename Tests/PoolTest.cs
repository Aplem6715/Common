using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Aplem.Common
{
    public class PoolTest
    {
        private Scene _scene;

        // A Test behaves as an ordinary method
        [Test]
        public void PoolTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PoolTestWithEnumeratorPasses()
        {
            var rents = new Stack<PoolTestObj>();
            var testObj = GameObject.Find("TestObj");
            var pool = new ManagedMonoPool<PoolTestObj>(testObj, testObj.transform, 5);

            // 取り出しテスト
            rents.Push(pool.Rent());
            rents.Push(pool.Rent());
            rents.Push(pool.Rent());
            Assert.IsTrue(pool.ActiveCount == rents.Count);
            Assert.IsTrue(pool.Capacity == 5);
            Assert.IsTrue(pool.PoolingCount == 2);

            // 上限以上取り出しテスト
            rents.Push(pool.Rent());
            rents.Push(pool.Rent());
            rents.Push(pool.Rent());
            rents.Push(pool.Rent());
            Assert.IsTrue(pool.ActiveCount == rents.Count);
            Assert.IsTrue(pool.Capacity == 7);
            Assert.IsTrue(pool.PoolingCount == 0);

            // 返却テスト
            for (int i = 0; i < 3; i++)
            {
                var obj = rents.Pop();
                ((IPoolableMono)obj).Return();
            }
            Assert.IsTrue(pool.ActiveCount == rents.Count);
            Assert.IsTrue(pool.Capacity == 7);
            Assert.IsTrue(pool.PoolingCount == 3);

            yield return null;
        }


        [UnitySetUp]
        public IEnumerator Setup()
        {
            _scene = EditorSceneManager.LoadSceneInPlayMode(
                "Assets/Project/Scripts/Common/Tests/PoolTest.unity",
                new LoadSceneParameters(LoadSceneMode.Additive)
            );
            yield return null;
            SceneManager.SetActiveScene(_scene);

            yield break;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            SceneManager.UnloadSceneAsync(_scene);
            yield break;
        }
    }
}
