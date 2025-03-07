using events;
using UnityEngine;

namespace events {
    public class CardDestroy : CardEvent {
        public CardDestroy(CardWrapper card) : base(card) {
        }
    }
}
