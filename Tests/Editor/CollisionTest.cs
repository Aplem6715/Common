using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Aplem.Common
{
    public class CollisionTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CircleLineTest()
        {
            // 始点と衝突
            var center = new Vector2(-0.5f, 0);
            var radius = 1;
            var start = center;
            var end = start + new Vector2(10, 10);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 終点と衝突
            center = new Vector2(10.5f, 0);
            radius = 1;
            start = new Vector2(0, 0);
            end = center;
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央上交差
            center = new Vector2(5, 1);
            radius = 2;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央下交差
            center = new Vector2(5, -1);
            radius = 2;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 始点と接触
            center = new Vector2(0, 1);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 終点と接触
            center = new Vector2(10, 1);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央接触
            center = new Vector2(5, 1);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央下接触
            center = new Vector2(5, -1);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsTrue(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 始点より左
            center = new Vector2(-1.1f, 0);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsFalse(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 終点より右
            center = new Vector2(11.1f, 0);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsFalse(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央上非接触
            center = new Vector2(5, 1.1f);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsFalse(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));

            // 中央下非接触
            center = new Vector2(5, -1.1f);
            radius = 1;
            start = new Vector2(0, 0);
            end = new Vector2(10, 0);
            Assert.IsFalse(CollisionUtil.CheckCollideCircleLine(center, radius, start, end));
        }
    }
}
