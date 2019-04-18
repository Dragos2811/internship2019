using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Card
    {
        public String Name { get; set; }
        public float Fee { get; set; }
        public DateTime ExpireDate { get; set; }
        public float Balance { get; set; }
        public Card(string Name,float Fee,DateTime ExpireDate)
        {
            this.Name = Name;
            this.Fee = Fee / 100;
            this.ExpireDate = ExpireDate;
            this.Balance = 0;
        }
        public void addBalance(float Balance)
        {
            this.Balance += Balance;
        }
        public bool transferBalance(Card c,float Balance)
        {
            float feeAmount = this.Fee * Balance;
            if (Balance <= this.Balance + feeAmount){
                c.Balance += Balance;
                this.Balance -= this.Balance + feeAmount;
                return true;
            }
            else
            {
                Console.WriteLine("insufficient funds"); //this can be replace with raising a custom error
                return false;
            }
        }
    }
    class Silver : Card
    {
        public Silver(DateTime ExpireDate) : base("Silver", 0.2f, ExpireDate)
        {
        }
    }
    class Gold : Card
    {
        public Gold(DateTime ExpireDate) : base("Gold", 0.1f, ExpireDate)
        {
        }
    }
    class Platinum : Card
    {
        public Platinum(DateTime ExpireDate) : base("Platinum", 0.3f, ExpireDate)
        {
        }
    }
    class Iridium : Card
    {
        public Iridium(DateTime ExpireDate) : base("Iridium", 0.2f, ExpireDate)
        {
        }
    }
    class Bronze : Card
    {
        public Bronze(DateTime ExpireDate) : base("Bronze", 0.5f, ExpireDate)
        {
        }
    }
    class Premium : Card
    {
        public Premium(DateTime ExpireDate) : base("Premium", 0.15f, ExpireDate)
        {
        }
    }
    class User
    {
        private List<Card> cards;
        public User()
        {
            this.cards = new List<Card>();
        }
        public User(List<Card> cards)
        {
            this.cards = new List<Card>();
            foreach(Card card in cards)
            {
                this.cards.Add(card);
            }
        }
        private void addCard(Card card)
        {
            this.cards.Add(card);
        }
        public bool addBalance(String CardName, float Balance)
        {
            bool done = false;
            foreach (Card c in this.cards)
            {
                if(c.Name == CardName)
                {
                    c.addBalance(Balance);
                    done = true;
                    break;
                }
            }
            if(done == false)
            {
                Console.WriteLine("There is no card of this type"); //this can be replace with raising a custom error
            }
            return done;
        }
        public Dictionary<Card, Tuple<float,float>> getCardsCost(float productCost,DateTime curentDate)
        {
            Dictionary<Card, Tuple<float, float>> d = new Dictionary<Card, Tuple<float, float>>();
            List<Card> validCards = getValidCards(curentDate);
            foreach (Card card in validCards)
            {
                d.Add(card, getCardCost(card, productCost, validCards.ToList()));
            }
            return d;
        }
        private Card getSmallestFeeCard(List<Card> cards)
        {
            float fee = 1.1f; //the highest fee that can exists is 100% -> 100/100 = 1
            Card bestCard = null;
            foreach(Card card in cards)
            {
                if(card.Fee < fee)
                {
                    bestCard = card;
                    fee = card.Fee;
                }
            }
            return bestCard;
        }
        public Tuple<float, float> getCardCost(Card card,float productCost,List<Card> validCards)
        {
            float fee;
            if (card.Balance >= productCost)
            {
                fee = productCost * card.Fee;
            }
            else
            {
                validCards.Remove(card);
                fee = productCost * card.Fee;
                float fictiveBalance = card.Balance;
                do
                {
                    Card fromCard = getSmallestFeeCard(validCards);
                    float needed = productCost - fictiveBalance;
                    float transferFee = needed * fromCard.Fee;
                    if (fromCard.Balance >= needed + transferFee)
                    {
                        fictiveBalance += needed;
                    }
                    else
                    {
                        transferFee = fromCard.Balance * fromCard.Fee;
                        fictiveBalance += fromCard.Balance - transferFee;
                        validCards.Remove(fromCard);
                    }
                    fee += transferFee;
                } while (fictiveBalance != productCost);
            }
            float tva = (productCost - fee) * 19 / 100;
            return new Tuple<float, float>(tva, fee);
        }
        public List<Card> getValidCards(DateTime currentDay)
        {
            List<Card> validCards = new List<Card>();
            foreach(Card card in cards)
            {
                if(DateTime.Compare(currentDay, card.ExpireDate) < 0)
                {
                    validCards.Add(card);
                }
            }
            return validCards;
        }
        
        public Card getCard(String Name)
        {
            foreach (Card card in this.cards)
            {
                if(card.Name == Name)
                {
                    return card;
                }
            }
            return null;
        }
        static void Main(string[] args)
        {
            User user = new User();
            DateTime currentDay = new DateTime(2019, 3, 19);
            
            user.addCard(new Silver(new DateTime(2020, 5, 23)));
            user.addBalance("Silver", 4000);
            user.addCard(new Gold(new DateTime(2018, 8, 15)));
            user.addBalance("Gold", 2000);
            user.addCard(new Platinum(new DateTime(2019, 3, 20)));
            user.addBalance("Platinum", 3000);
            user.addCard(new Iridium(new DateTime(2020, 5, 23)));
            user.addBalance("Iridium", 5000);
            user.addCard(new Bronze(new DateTime(2019, 7, 15)));
            user.addBalance("Bronze", 2500);
            user.addCard(new Premium(new DateTime(2019, 8, 20)));
            user.addBalance("Premium", 2000);
            List<Card> validCards = user.getValidCards(currentDay);
            Tuple<float, float> tvafee = user.getCardCost(user.getCard("Iridium"), 10000, validCards);
            Dictionary<Card, Tuple<float, float>> tvafee1 = user.getCardsCost(10000, currentDay);
            foreach(var item in tvafee1)
            {
                Console.Write(item.Key.Name + ": ");
                Console.WriteLine("\tTva: "+ item.Value.Item1 + "\tFee:"+item.Value.Item2);
            }
        }
    }
}
