using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using EleCho.WpfSuite.Media.Animation;

namespace OnePageLimitScrollingDemo.Components
{
    public class OnePageLimitScrollViewer : ScrollViewer
    {
        private readonly ValueAnimator<double> _animatedVerticalOffset;
        private readonly DispatcherTimer _scrollFixTimer;

        private DateTime _lastScrollTime;

        public OnePageLimitScrollViewer()
        {
            _animatedVerticalOffset = new ValueAnimator<double>();
            _animatedVerticalOffset.Frequency = 2;
            _animatedVerticalOffset.AnimatedValueChanged += (s, e) =>
            {
                ScrollToVerticalOffset(_animatedVerticalOffset.AnimatedValue);
            };

            _scrollFixTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, ScrollFixTimer_Tick, Dispatcher);
        }

        private void ScrollFixTimer_Tick(object? sender, EventArgs e)
        {
            var pageSize = ViewportHeight;

            if (_animatedVerticalOffset.Value < pageSize &&
                _animatedVerticalOffset.Value != 0 &&
                (DateTime.Now - _lastScrollTime) > TimeSpan.FromMilliseconds(300))
            {
                if (_animatedVerticalOffset.Value < pageSize / 2)
                {
                    _animatedVerticalOffset.Value = 0;
                }
                else
                {
                    _animatedVerticalOffset.Value = pageSize;
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var change = - e.Delta;

            var pageSize = ViewportHeight;
            var oldOffset = _animatedVerticalOffset.Value;
            var newOffset = oldOffset + change;

            if (newOffset < 0)
            {
                newOffset = 0;
            }

            if (newOffset > ScrollableHeight)
            {
                newOffset = ScrollableHeight;
            }

            if (newOffset > pageSize &&
                _animatedVerticalOffset.AnimatedValue < pageSize - 1)
            {
                newOffset = pageSize;
            }

            if (newOffset < pageSize &&
                _animatedVerticalOffset.AnimatedValue > pageSize + 1)
            {
                newOffset = pageSize;
            }

            _animatedVerticalOffset.Value = newOffset;
            _lastScrollTime = DateTime.Now;

            //base.OnMouseWheel(e);
        }
    }
}
