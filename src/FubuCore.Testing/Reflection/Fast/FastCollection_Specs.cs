// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using FubuCore.Reflection.Fast;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Fast
{
    [TestFixture]
    public class FastCollection_Specs
    {
        [Test]
        public void Generic_Add()
        {
            IList<int> b = new List<int>();

            var fc = new FastCollection<IList<int>, int>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);
        }

        [Test]
        public void Generic_Remove()
        {
            IList<int> b = new List<int>();

            var fc = new FastCollection<IList<int>, int>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);

            fc.Remove(b, 2);

            Assert.AreEqual(0, b.Count);
        }

        [Test]
        public void Add()
        {
            IList b = new List<int>();

            var fc = new FastCollection<IList>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);
        }

        [Test]
        public void Remove()
        {
            IList b = new List<int>();

            var fc = new FastCollection<IList>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);

            fc.Remove(b, 2);

            Assert.AreEqual(0, b.Count);
        }
    }
}