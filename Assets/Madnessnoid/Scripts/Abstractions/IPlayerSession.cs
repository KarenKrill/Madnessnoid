#nullable enable

using System;

namespace Madnessnoid.Abstractions
{
    public interface IPlayerSession
    {
        public int Money { get; }

        public event Action? MoneyChanged;

        public void AddMoney(int money);
        public void RemoveMoney(int money);
    }
}
