using System;

namespace Madnessnoid
{
    using Abstractions;

    public class PlayerSession : IPlayerSession
    {
        public int Money { get; private set; }

        public event Action MoneyChanged;

        public void AddMoney(int money)
        {
            if (money > 0)
            {
                Money += money;
                MoneyChanged?.Invoke();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(money), money, "The amount of money added must be greater than 0");
            }
        }

        public void RemoveMoney(int money)
        {
            if (money <= Money)
            {
                Money -= money;
                MoneyChanged?.Invoke();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(money), money, "The amount of money removed must be less than current money's amount");
            }
        }
    }
}
