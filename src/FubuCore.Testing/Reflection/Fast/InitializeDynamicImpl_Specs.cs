﻿// Copyright 2007-2010 The Apache Software Foundation.
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

using System;
using FubuCore.Reflection.Fast;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Fast
{
    [TestFixture]
    public class When_initializing_a_dynamic_object_implementation
    {
        DateTime _dateTimeValue = DateTime.UtcNow;
        int _intValue = 42;
        string _stringValue = "Johnson";
        ISubject _subject;

        [SetUp]
        public void Initializing_a_dynamic_object_implementation()
        {
            _subject = (ISubject)typeof(ISubject).InitializeProxy(new
                {
                    IntValue = _intValue,
                    StringValue = _stringValue,
                    BooleanValue = true,
                    DateTimeValue = _dateTimeValue,
                });
        }

        [Test]
        public void Should_set_a_value_type_properly()
        {
            _subject.IntValue.ShouldEqual(_intValue);
        }

        [Test]
        public void Should_set_a_string_value()
        {
            _subject.StringValue.ShouldEqual(_stringValue);
        }

        [Test]
        public void Should_set_a_boolean_value()
        {
            _subject.BooleanValue.ShouldBeTrue();
        }

        [Test]
        public void Should_set_a_nullable_value_type()
        {
            _subject.DateTimeValue.HasValue.ShouldBeTrue();
            _subject.DateTimeValue.Value.ShouldEqual(_dateTimeValue);
        }


        public interface ISubject
        {
            int IntValue { get; }
            string StringValue { get; }
            bool BooleanValue { get; }
            DateTime? DateTimeValue { get; }
        }
    }
}