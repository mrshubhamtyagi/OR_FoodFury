using System;

namespace FoodFury
{
    public interface IScreen
    {
        public void Show(Action _callback = null);
        public void Hide(Action _callback = null);
    }
}
