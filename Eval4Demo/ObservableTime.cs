using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eval4DemoCSharp
{
    static class ObservableTime
    {
        static System.Windows.Forms.Timer timer;

        static ObservableTime()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Enabled = true;
        }

        public static IObservable<int> Seconds
        {
            get
            {
                var result = new Observable<int>();
                var timerEventHandler = new EventHandler((sender, e) =>
                {
                    result.RaiseNext((int)DateTime.Now.Ticks);
                });
                timer.Tick += timerEventHandler;
                return result;
            }
        }

    }

    class Observable<T> : IObservable<T>
    {
        private List<ObservableObserver<T>> mObservers = new List<ObservableObserver<T>>();
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var result = new ObservableObserver<T>(observer, this);
            mObservers.Add(result);
            return result;
        }

        public Observable()
        {
        }

        internal void RaiseNext(T value)
        {
            foreach (var i in mObservers)
            {
                i.RaiseNext(value);
            }
        }

        internal void OnDispose(ObservableObserver<T> observableObserver)
        {
            mObservers.Remove(observableObserver);
        }
    }

    class Observer<T> : IObserver<T>
    {
        Action<T> OnNext;
        Action OnCompleted;
        Action<Exception> OnError;

        public Observer(
            Action<T> onNext = null,
            Action onCompleted = null,
            Action<Exception> onError = null)
        {
            this.OnCompleted = onCompleted;
            this.OnError = onError;
            this.OnNext = onNext;
        }

        void IObserver<T>.OnCompleted()
        {
            if (OnCompleted != null) OnCompleted();
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (OnError != null) OnError(error);
        }

        void IObserver<T>.OnNext(T value)
        {
            if (OnNext != null) OnNext(value);
        }
    }

    class ObservableObserver<T> : IDisposable
    {
        private IObserver<T> observer;
        private Observable<T> observable;


        public ObservableObserver(IObserver<T> observer, Observable<T> observable)
        {
            this.observer = observer;
            this.observable = observable;
        }

        public void Dispose()
        {
            observable.OnDispose(this);
        }

        internal void RaiseNext(T value)
        {
            observer.OnNext(value);
        }
    }
}
