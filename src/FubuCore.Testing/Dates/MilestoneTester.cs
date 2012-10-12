﻿using System;
using FubuCore.Dates;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Dates
{
    [TestFixture]
    public class MilestoneTester
    {
        [Test]
        public void clear()
        {
            var time = DateTime.Today.AddHours(8);

            var milestone1 = new Milestone(time);
            milestone1.IsTrue.ShouldBeTrue();

            milestone1.Clear();

            milestone1.Timestamp.ShouldBeNull();
            milestone1.IsTrue.ShouldBeFalse();
        }

        [Test]
        public void equals_positive_with_matching_times()
        {
            var time = DateTime.Today.AddHours(8);

            var milestone1 = new Milestone(time);
            var milestone2 = new Milestone(time);

            milestone1.ShouldEqual(milestone2);
            milestone2.ShouldEqual(milestone1);
        }

        [Test]
        public void equals_positive_with_both_being_null()
        {
            var milestone1 = new Milestone();
            var milestone2 = new Milestone();

            milestone1.ShouldEqual(milestone2);
            milestone2.ShouldEqual(milestone1);
        }

        [Test]
        public void equals_negative()
        {
            var time = DateTime.Today.AddHours(8);

            var milestone1 = new Milestone(time);
            var milestone2 = new Milestone();
            var milestone3 = new Milestone(time.AddHours(1));

            milestone1.ShouldNotEqual(milestone2);
            milestone1.ShouldNotEqual(milestone3);
            milestone2.ShouldNotEqual(milestone1);
            milestone2.ShouldNotEqual(milestone3);
            milestone3.ShouldNotEqual(milestone1);
            milestone3.ShouldNotEqual(milestone2);
        }

        [Test]
        public void is_true_is_false_when_no_timestamp()
        {
            new Milestone().IsTrue.ShouldBeFalse();
            new Milestone().IsFalse.ShouldBeTrue();
        }

        [Test]
        public void is_true_when_timestamp_is_captured()
        {
            new Milestone(DateTime.Now).IsTrue.ShouldBeTrue();
        }

        [Test]
        public void capture_stores_the_milestone_timestamp()
        {
            var time = DateTime.Today.AddHours(8);
            new Milestone().Capture(time).Timestamp.ShouldEqual(time.ToUniversalTime());
        }

        [Test]
        public void happened_before()
        {
            var time = DateTime.Today.AddHours(8).ToUniversalTime();
            var milestone = new Milestone(time);

            milestone.HappenedBefore(time.AddMinutes(-1)).ShouldBeFalse();
            milestone.HappenedBefore(time.AddDays(-1)).ShouldBeFalse();
            milestone.HappenedBefore(time.AddDays(1)).ShouldBeTrue();
        }
    }
}