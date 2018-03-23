using System;
using Xunit;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using System.Reactive;

namespace Rx
{
    public class ObservableTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Start(int seed)
        {
            var scheduler = new TestScheduler();
            var source = scheduler.CreateColdObservable(
                new Recorded<Notification<int>>(100, Notification.CreateOnNext(seed)),
                new Recorded<Notification<int>>(200, Notification.CreateOnCompleted<int>()));

            var results = scheduler.CreateObserver<int>();

            source.Subscribe(results);

            scheduler.Start();

            results.Messages.AssertEqual(
                new Recorded<Notification<int>>(100, Notification.CreateOnNext(seed)),
                new Recorded<Notification<int>>(200, Notification.CreateOnCompleted<int>())
            );
        }
    }
}
