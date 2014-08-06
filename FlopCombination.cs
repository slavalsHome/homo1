using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common
{
    public class FlopCombination
    {
        public FlopCombination()
        {
            Dro1 = new List<FCombinaton>();
        }

        public string Name123 { get; set; }

        public FCombinaton Combinaton { get; set; }

        public List<FCombinaton> Dro { get; set; }

        public enum FCombinaton
        {
            StreetFlash,
            
            Care,
            WeakCare, // когда на доске 4 карты одного достоинства, а у тебя не туз
            
            FullHouse,
            //FullHouseLikeTopPair, // Когда на столе две пары, а у тебя замыкатель к младшей из них
            WeakFullHouse, // когда на столе уже лежит трипс а у тебя маленькая пара 
            
            Flash, // Все карты одной масти
            WeakFlash, // 4 карты одной масти на доске и у тебя не туз и не король
            
            FlashDro, // 4 карты одной масти из которых две у тебя на руках
            WeakFlashDroTop, // 4 карты одной масти из которых одна у тебя на руках, но это туз или король
            WeakFlashDro, // 4 карты одной масти из которых одна у тебя на руках, и это НЕ туз и НЕ король

            Street, // Пять карт подряд
            WeakStreet, // одна определенная карта дает стрит старше твоего

            StreetDro, // Двухстроронний стрит дро
            WeakStreetDro, // Двухсторонний стрит дро в котором у тебя одна карта и та снизу и не десятка (верхняя карта тебя не устраивает, так как на борде будет лежать 4 подряд а у тебя не сверху)
            GatShot, // одной карты тебе не хватает до стрита
            WeakGatShot, // стрит дро без карт, или гэтшот закрывающийся в слабый стрит
            
            Threeps, // когда у тебя пара дающая с картой флопа тройку
            ThreepsWithOne, // Слабая тройка, когда на флопе две карты а у тебя третья
            WeakThreeps, // Это когда три карты на борде а у тебя к ним ничего нет
            
            TwoPair, // когда у тебя две карты совпадают с двумя картами на борде
            TwoPairLikePair, // пара на борде а твоя пара старше всех карт  Когда на флопе пара, а твоя карта составляет пару старшей на доске (не в паре)
            WeakTwoPair, // Когда на флопе пара, а твоя карта составляет пару НЕ старшей на доске (не в паре) или вообще твои карты не к парам
            
            OverPair, // Овер пара
            TopPair, // Топ пара
            WeakPair, // Не топ пара
            
            OverCard, // Катра старше карт флопа
            OverCards, // Две карты старше карт флопа

            None
        }


        public enum FlopLevel
        {
            Monstr = 0,
            OverPair = 1,
            MonstrDro = 2,
            TopPair = 3,
            Dro = 4, // Флэш дро или стрит дро
            WeakDro = 5, //гэт шот или оверкарты
            Musor = 6, // мусор
            
        }
        public static FlopLevel GetLevel(FlopCombination cmb)
        {
            FlopLevel result = FlopLevel.Musor;

            if (cmb.Combinaton == FCombinaton.StreetFlash ||
                cmb.Combinaton == FCombinaton.Care ||
                cmb.Combinaton == FCombinaton.FullHouse ||
                cmb.Combinaton == FCombinaton.Flash ||
                cmb.Combinaton == FCombinaton.Street ||
                cmb.Combinaton == FCombinaton.Threeps ||
                cmb.Combinaton == FCombinaton.ThreepsWithOne ||
                cmb.Combinaton == FCombinaton.TwoPair)
            {
                result = FlopLevel.Monstr;
            }

            if (result == FlopLevel.Musor)
            {
                if (cmb.Combinaton == FCombinaton.OverPair)
                {
                    result = FlopLevel.OverPair;
                }
            }

            if (result == FlopLevel.Musor)
            {
                // проверяем на монстр дро
                bool flashDro = false;
                bool streetDro = false;
                bool topPair = cmb.Combinaton == FCombinaton.TopPair;

                foreach (FCombinaton fCombinaton in cmb.Dro)
                {
                    if (fCombinaton == FCombinaton.StreetDro ||
                        fCombinaton == FCombinaton.WeakStreetDro ||
                        fCombinaton == FCombinaton.GatShot)
                    {
                        streetDro = true;
                    }

                    if (fCombinaton == FCombinaton.FlashDro ||
                        fCombinaton == FCombinaton.WeakFlashDroTop)
                    {
                        flashDro = true;
                    }
                }

                if (flashDro && (streetDro || topPair))
                {
                    result = FlopLevel.MonstrDro;
                }
            }

            if (result == FlopLevel.Musor)
            {
                // проверяем на топ пару
                if (cmb.Combinaton == FCombinaton.TopPair ||
                    cmb.Combinaton == FCombinaton.TwoPairLikePair)
                {
                    result = FlopLevel.TopPair;
                }
            }

            if (result == FlopLevel.Musor)
            {
                // проверяем дро
                bool dro = false;
                foreach (FCombinaton fCombinaton in cmb.Dro)
                {
                    if (fCombinaton == FCombinaton.StreetDro ||
                        fCombinaton == FCombinaton.FlashDro ||
                        fCombinaton == FCombinaton.WeakFlashDroTop)
                    {
                        dro = true;
                    }
                }

                if (dro)
                    result = FlopLevel.Dro;
            }

            if (result == FlopLevel.Musor)
            {
                // проверяем дро
                bool dro = false;
                foreach (FCombinaton fCombinaton in cmb.Dro)
                {
                    if (fCombinaton == FCombinaton.WeakStreetDro ||
                        fCombinaton == FCombinaton.GatShot)
                    {
                        dro = true;
                    }
                }

                if (dro || cmb.Combinaton == FCombinaton.OverCards)
                    result = FlopLevel.WeakDro;
            }

            return result;
        }


        #region Статические методы

        /// <summary>
        /// Возвращает что за комбинация из 5 карт
        /// </summary>
        public static FlopCombination GetCombination(List<Card> myCard, List<Card> flopCard)
        {
            FlopCombination cmb = null;
            List<FCombinaton> dro = new List<FCombinaton>();
            if (cmb == null)
                cmb = GetStreetFlash(myCard, flopCard);
            if (cmb == null)
                cmb = GetCare(myCard, flopCard);
            if (cmb == null)
                cmb = GetFullHouse(myCard, flopCard);
            if (cmb == null)
            {
                cmb = GetFlash(myCard, flopCard);

                if (cmb != null)
                {
                    if (cmb.Combinaton != FCombinaton.Flash &&
                        cmb.Combinaton != FCombinaton.WeakFlash)
                    {
                        dro.Add(cmb.Combinaton);
                        cmb = null;
                    }
                }
            }
            if (cmb == null)
            {
                cmb = GetStreet(myCard2, flopCard2);

                if (cmb != null)
                {
                    if (cmb.Combinaton != FCombinaton.Street &&
                        cmb.Combinaton != FCombinaton.WeakStreet)
                    {
                        dro.Add(cmb.Combinaton);
                        cmb = null;
                    }
                }
            }

            if (cmb == null)
                cmb = GetThreeps(myCard, flopCard);
            if (cmb == null)
                cmb = GetTwoPair(myCard, flopCard);
            if (cmb == null)
                cmb = GetPair(myCard, flopCard);
            if (cmb == null)
                cmb = GetOverCard(myCard, flopCard);

            if (cmb == null)
            {
                cmb = new FlopCombination();
                cmb.Name = "Ничего";
                cmb.Combinaton = FCombinaton.None;
            }

            cmb.Dro.AddRange(dro);

            return cmb;
        }

        private static FlopCombination GetStreetFlash(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            // Проверяем все ли одной масти
            var dict = new Dictionary<int, List<int>>();

            for (int i = 0; i < card.Count; i++)
            {
                var mast = card[i].Mast;
                if (!dict.ContainsKey(mast))
                {
                    dict.Add(mast, new List<int>());
                }
                
                dict[mast].Add(card[i].Dost);
            }

            foreach (List<int> dosts in dict.Values)
            {
                if (dosts.Count >= 5)
                {
                    dosts.Sort();
                    // Есть пять карт одной масти
                    int podryad = 0;
                    for (int i = 0; i < dosts.Count - 1; i++)
                    {
                        if (i == 0 && dosts[0] == 0 && dosts[dosts.Count - 1] == 12)
                        {
                            // Если это первая карта и она двойка, а последняя карта туз
                            podryad++;
                        }

                        if (dosts[i + 1] - dosts[i] == 1)
                        {
                            podryad++;
                        }
                        else
                        {
                            podryad = 0;
                        }

                        if (podryad + 1 == 5)
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "СтритФлэш";
                            cmb.Combinaton = FCombinaton.StreetFlash;
                        }
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает каре
        /// </summary>
        /// <returns></returns>
        private static FlopCombination GetCare(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (dict.ContainsKey(dost))
                {
                    dict[dost] += 1;
                }
                else
                {
                    dict.Add(dost, 1);
                }
            }
            
            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (pair.Value == 4)
                {
                    // хотя бы одна из твоих карт должна быть этого достоинства
                    if (myCard[0].Dost == pair.Key || myCard[1].Dost == pair.Key)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Каре";
                        cmb.Combinaton = FCombinaton.Care;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Каре (слабое)";
                        cmb.Combinaton = FCombinaton.WeakCare;
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает фулхаус
        /// </summary>
        private static FlopCombination GetFullHouse(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (dict.ContainsKey(dost))
                {
                    dict[dost] += 1;
                }
                else
                {
                    dict.Add(dost, 1);
                }
            }


            List<int> dost2 = new List<int>();
            List<int> dost3 = new List<int>();
             
            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (pair.Value == 3)
                {
                    dost3.Add(pair.Key);
                }
                if (pair.Value == 2)
                {
                    dost2.Add(pair.Key);
                }
            }

            if (dost3.Count > 0)
            {
                // Есть трипс
                if (dost3.Count > 1)
                {
                    // Два раза по три

                    bool weak = false;

                    int parentDost = dost3[0] > dost3[1] ? dost3[0] : dost3[1];
                    int otherDost = dost3[0] < dost3[1] ? dost3[0] : dost3[1];
                    if (myCard[0].Dost != parentDost && myCard[1].Dost != parentDost)
                    {
                        // у тебя нет карт в трипсе
                        if (myCard[0].Dost != myCard[1].Dost)
                        {
                            // Значит на столе фул уже есть
                            weak = true;
                        }
                        else
                        {
                            // твоя пара ниже десятки
                            if (otherDost < 8)
                            {
                                // Твоя пара меньше десятки
                                weak = true;
                            }
                            else
                            {
                                // вдруг есть карта старше твоей пары
                                bool ffind = false;
                                foreach (Card fcard in flopCard)
                                {
                                    if (fcard.Dost != parentDost && fcard.Dost != otherDost
                                        && fcard.Dost > otherDost)
                                    {
                                        ffind = true;
                                    }
                                }

                                if (ffind)
                                {
                                    weak = true;
                                }
                            }
                        }
                    }

                    if (weak)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Фулхаус (слабый)";
                        cmb.Combinaton = FCombinaton.WeakFullHouse;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Фулхаус";
                        cmb.Combinaton = FCombinaton.FullHouse;
                    }
                }
                else if (dost2.Count > 0)
                {
                    int parentDost = dost3[0];
                    int otherDost = dost2[0];

                    bool weak = false;

                    if (myCard[0].Dost != parentDost && myCard[1].Dost != parentDost)
                    {
                        // у тебя нет карт в трипсе
                        // твоя пара ниже десятки
                        if (otherDost < 8)
                        {
                            // Твоя пара меньше десятки
                            weak = true;
                        }
                        else
                        {
                            // вдруг есть карта старше твоей пары
                            bool ffind = false;
                            foreach (Card fcard in flopCard)
                            {
                                if (fcard.Dost != parentDost && fcard.Dost != otherDost
                                    && fcard.Dost > otherDost)
                                {
                                    ffind = true;
                                }
                            }

                            if (ffind)
                            {
                                weak = true;
                            }
                        }
                    }
                    else
                    {
                        // Если в трипсе но на борде две пары и трипс ты собрал к младшей
                        if (myCard[0].Dost != myCard[1].Dost) // Если в трипсе только одна твоя карта
                        {
                            if (myCard[0].Dost != otherDost && myCard[1].Dost != otherDost) // А в паре нет твоей карты
                            {
                                if (otherDost > parentDost) // и пара достоинством старше трипса
                                {
                                    weak = true;
                                }
                            }
                        }
                    }

                    if (weak)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Фулхаус (слабый)";
                        cmb.Combinaton = FCombinaton.WeakFullHouse;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Фулхаус";
                        cmb.Combinaton = FCombinaton.FullHouse;
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает флэш
        /// </summary>
        private static FlopCombination GetFlash(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            // Проверяем все ли одной масти
            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var mast = card[i].Mast;
                if (dict.ContainsKey(mast))
                {
                    dict[mast] += 1;
                }
                else
                {
                    dict.Add(mast, 1);
                }
            }

            foreach (KeyValuePair<int, int> keyValuePair in dict)
            {
                if (keyValuePair.Value >= 5)
                {
                    // Это флэш
                    bool weak = false;

                    int myCnt = 0;
                    int myDost = -1;
                    if (myCard[0].Mast == keyValuePair.Key)
                    {
                        myCnt++;
                        myDost = myCard[0].Dost;
                    }
                    if (myCard[1].Mast == keyValuePair.Key)
                    {
                        myCnt++;
                        if (myCard[1].Dost > myDost)
                            myDost = myCard[1].Dost;
                    }

                    if (keyValuePair.Value - myCnt >= 4)
                    {
                        if (myDost < 11)
                        {
                            weak = true;
                        }
                    }

                    if (weak)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Флэш (слабый)";
                        cmb.Combinaton = FCombinaton.WeakFlash;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Флэш";
                        cmb.Combinaton = FCombinaton.Flash;
                    }

                }
            }

            if (cmb == null)
            {
                foreach (KeyValuePair<int, int> keyValuePair in dict)
                {
                    if (keyValuePair.Value == 4)
                    {
                        // Это флэш-дро
                        int myCnt = 0;
                        int myDost = -1;
                        if (myCard[0].Mast == keyValuePair.Key)
                        {
                            myCnt++;
                            myDost = myCard[0].Dost;
                        }
                        if (myCard[1].Mast == keyValuePair.Key)
                        {
                            myCnt++;
                            if (myCard[1].Dost > myDost)
                                myDost = myCard[1].Dost;
                        }

                        if (myCnt == 2)
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Флэш-дро";
                            cmb.Combinaton = FCombinaton.FlashDro;
                        }
                        else if (myCnt == 1)
                        {
                            if (myDost < 11)
                            {
                                cmb = new FlopCombination();
                                cmb.Name = "Флэш-дро (слабое)";
                                cmb.Combinaton = FCombinaton.WeakFlashDro;
                            }
                            else
                            {
                                cmb = new FlopCombination();
                                cmb.Name = "Флэш-дро (с одной сильной картой)";
                                cmb.Combinaton = FCombinaton.WeakFlashDroTop;
                            }
                        }
                        else
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Флэш-дро (слабое)";
                            cmb.Combinaton = FCombinaton.WeakFlashDro;
                        }
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает стрит
        /// </summary>
        private static FlopCombination GetStreet(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            List<int> dosts = new List<int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (!dosts.Contains(dost))
                {
                    dosts.Add(dost);
                }
            }

            dosts.Sort();
            int podryad = 0;
            int streetEndWith = -1;
            for (int i = 0; i < dosts.Count - 1; i++)
            {
                if (i == 0 && dosts[0] == 0 && dosts[dosts.Count - 1] == 12)
                {
                    // Если это первая карта и она двойка, а последняя карта туз
                    podryad++;
                }

                if (dosts[i + 1] - dosts[i] == 1)
                {
                    podryad++;
                }
                else
                {
                    podryad = 0;
                }

                if (podryad >= 4)
                {
                    streetEndWith = dosts[i + 1];
                }
            }

            if (streetEndWith > 0)
            {
                // Если у тебя 2 карты от стрита и их нет на доске
                // Если у тебя одна карта от стрита и она не в самом начале - то стрит не слабый (или крта в начале стриа но эта карта - десятка)
                int myCardCount = 0;
                int myCardIndex = -1;
                for (int i = 0; i < 5; i++)
                {
                    int dost = streetEndWith - (4 - i);
                    if (dost == -1)
                        dost = 12;

                    bool fFind = false;
                    foreach (Card card1 in flopCard)
                    {
                        if (card1.Dost == dost)
                            fFind = true;
                    }

                    if (!fFind)
                    {
                        myCardIndex = i;
                        myCardCount++;
                    }
                }

                if ((myCardIndex == 0 && streetEndWith != 12) || myCardIndex < 0)
                {
                    cmb = new FlopCombination();
                    cmb.Name = "Стрит (слабый)";
                    cmb.Combinaton = FCombinaton.WeakStreet;
                }
                else
                {
                    cmb = new FlopCombination();
                    cmb.Name = "Стрит";
                    cmb.Combinaton = FCombinaton.Street;
                }
            }

            if (cmb == null)
            {
                // стрит дро
                int overCard = 0; // через карту назад сколько было подряд
                podryad = 0;

                int streetDroEndWith = -1;

                Dictionary<int,int> gatShotDict = new Dictionary<int, int>();

                for (int i = 0; i < dosts.Count - 1; i++)
                {
                    if (i == 0 && dosts[0] == 0 && dosts[dosts.Count - 1] == 12)
                    {
                        // Если это первая карта и она двойка, а последняя карта туз
                        podryad++;
                    }

                    if (i == 0 && dosts[0] == 1 && dosts[dosts.Count - 1] == 12)
                    {
                        // Если это первая карта и она тройка, а последняя карта туз
                        overCard = 1;
                    }

                    if (dosts[i + 1] - dosts[i] == 1)
                    {
                        podryad++;
                    }
                    else if (dosts[i + 1] - dosts[i] == 2)
                    {
                        overCard = 1 + podryad;
                        podryad = 0;
                    }
                    else
                    {
                        podryad = 0;
                        overCard = 0;
                    }

                    if (podryad + 1 + 1 >= 5)
                    {
                        // Это четыре карты подряд (двухсторонний)
                        streetDroEndWith = dosts[i + 1];
                    }
                    else if (overCard + podryad + 2 >= 5)
                    {
                        // Это гатшот с дыркой
                        // Записываем пару: дырка - чем заканчивается гэт шот
                        int endWith = dosts[i + 1];
                        int dirka = dosts[i + 1] -  (podryad  + 1);

                        if (gatShotDict.ContainsKey(dirka))
                            gatShotDict[dirka] = endWith;
                        else
                            gatShotDict.Add(dirka, endWith);
                    }
                }

                if (streetDroEndWith > 0)
                {
                    // Это стрит дро
                    int myCardIndex = -1;
                    for (int i = 0; i < 4; i++)
                    {
                        int dost = streetDroEndWith - (3 - i);
                        if (dost == -1)
                            dost = 12;
                        bool fFind = false;
                        foreach (Card card1 in flopCard)
                        {
                            if (card1.Dost == dost)
                                fFind = true;
                        }

                        if (!fFind)
                        {
                            myCardIndex = i;
                        }
                    }

                    if (myCardIndex < 0) // нет карт в стрит-дро
                    {
                        //это фигня а не стрит дро
                        cmb = new FlopCombination();
                        cmb.Name = "Стрит-дро (без карт)";
                        cmb.Combinaton = FCombinaton.WeakGatShot;

                    }
                    else if (streetDroEndWith == 12 || streetDroEndWith == 3) // стрит дро заканчивается тузом или четверкой 
                    {
                        // Это гэтшот
                        if (myCardIndex == 0)
                        {
                            // на слабый стрит
                            cmb = new FlopCombination();
                            cmb.Name = "Гэт-шот (слабый)";
                            cmb.Combinaton = FCombinaton.WeakGatShot;
                        }
                        else
                        {
                            // обычный гэтшот
                            cmb = new FlopCombination();
                            cmb.Name = "Гэт-шот (слабый)";
                            cmb.Combinaton = FCombinaton.GatShot;
                        }
                    }
                    else if (myCardIndex == 0)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Стрит-дро (слабый)";
                        cmb.Combinaton = FCombinaton.WeakStreetDro;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Стрит-дро";
                        cmb.Combinaton = FCombinaton.StreetDro;
                    }
                }
                else if (gatShotDict.Count == 2)
                {
                    // Двойной гэтшот мать его
                    bool ffirst = true;
                    bool hasone = false;
                    bool hastwo = false;
                    foreach (KeyValuePair<int, int> keyValuePair in gatShotDict)
                    {
                        int myCardIndex = -1;
                        for (int i = 0; i < 5; i++)
                        {
                            int dost = keyValuePair.Value - (4 - i);
                            if (dost == -1)
                                dost = 12;

                            if (dost == keyValuePair.Key)
                                continue;

                            bool fFind = false;
                            foreach (Card card1 in flopCard)
                            {
                                if (card1.Dost == dost)
                                    fFind = true;
                            }

                            if (!fFind)
                            {
                                myCardIndex = i;
                            }
                        }

                        if (ffirst)
                            hasone = myCardIndex > 0 || (myCardIndex == 0 && keyValuePair.Value == 12);
                        else
                            hastwo = myCardIndex > 0 || (myCardIndex == 0 && keyValuePair.Value == 12);

                        ffirst = false;
                    }

                    if (hasone && hastwo)
                    {
                        // Двойной гэт шот
                        cmb = new FlopCombination();
                        cmb.Name = "Стрит-дро";
                        cmb.Combinaton = FCombinaton.StreetDro;
                    }
                    else if (!hasone && !hastwo)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Стрит-дро (без карт или слабый)";
                        cmb.Combinaton = FCombinaton.WeakGatShot;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Гат-шот";
                        cmb.Combinaton = FCombinaton.GatShot;
                    }
                }
                else if (gatShotDict.Count == 1)
                {
                    foreach (KeyValuePair<int, int> keyValuePair in gatShotDict)
                    {
                        int myCardIndex = -1;
                        for (int i = 0; i < 5; i++)
                        {
                            int dost = keyValuePair.Value - (4 - i);
                            if (dost == -1)
                                dost = 12;

                            if (dost == keyValuePair.Key)
                                continue;

                            bool fFind = false;
                            foreach (Card card1 in flopCard)
                            {
                                if (card1.Dost == dost)
                                    fFind = true;
                            }

                            if (!fFind)
                            {
                                myCardIndex = i;
                            }
                        }

                        if (myCardIndex > 0 || (myCardIndex == 0 && keyValuePair.Value == 12))
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Гат-шот";
                            cmb.Combinaton = FCombinaton.GatShot;
                        }
                        else
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Стрит-дро (без карт или слабый)";
                            cmb.Combinaton = FCombinaton.WeakGatShot;
                        }
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает тройку
        /// </summary>
        /// <returns></returns>
        private static FlopCombination GetThreeps(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);


            FlopCombination cmb = null;

            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (dict.ContainsKey(dost))
                {
                    dict[dost] += 1;
                }
                else
                {
                    dict.Add(dost, 1);
                }
            }

            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (pair.Value == 3)
                {
                    // Если обе твоих карты такого достоинства
                    if (myCard[0].Dost == pair.Key && myCard[1].Dost == pair.Key)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Трипс";
                        cmb.Combinaton = FCombinaton.Threeps;
                    }
                    else if (myCard[0].Dost == pair.Key || myCard[1].Dost == pair.Key)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Трипс (с одной картой)";
                        cmb.Combinaton = FCombinaton.ThreepsWithOne;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Трипс (без карт)";
                        cmb.Combinaton = FCombinaton.WeakThreeps;
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает две пары
        /// </summary>
        private static FlopCombination GetTwoPair(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (dict.ContainsKey(dost))
                {
                    dict[dost] += 1;
                }
                else
                {
                    dict.Add(dost, 1);
                }
            }


            List<int> dost2 = new List<int>();

            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (pair.Value == 2)
                {
                    dost2.Add(pair.Key);
                }
            }

            if (dost2.Count > 1)
            {
                // Есть две пары
                dost2.Sort();
                int parentDost = dost2[dost2.Count - 1];
                int otherDost = dost2[dost2.Count - 2];

                bool fparent = false;
                bool fother = false;

                if (myCard[0].Dost == parentDost || myCard[1].Dost == parentDost)
                    fparent = true;
                if (myCard[0].Dost == otherDost || myCard[1].Dost == otherDost)
                    fother = true;


                if (fparent && fother)
                {
                    cmb = new FlopCombination();
                    cmb.Name = "Две пары";
                    cmb.Combinaton = FCombinaton.TwoPair;
                }
                else if (!fparent && !fother)
                {
                    cmb = new FlopCombination();
                    cmb.Name = "Две пары (слабые)";
                    cmb.Combinaton = FCombinaton.WeakTwoPair;
                }
                else
                {
                    bool weak = false;

                    if (fparent && parentDost < 8) // Пара ниже десятки
                        weak = true;

                    if (fother && otherDost < 8) // Пара ниже десятки
                        weak = true; 

                    if (!weak)
                    {
                        if (fparent)
                        {
                            foreach (Card card1 in flopCard)
                            {
                                if (card1.Dost != parentDost
                                    && card1.Dost != otherDost)
                                {
                                    if (card1.Dost > parentDost) // есть карта выше твоей пары
                                    {
                                        weak = true;
                                    }
                                }
                            }
                        }

                        if (fother)
                        {
                            foreach (Card card1 in flopCard)
                            {
                                if (card1.Dost != parentDost
                                    && card1.Dost != otherDost)
                                {
                                    if (card1.Dost > otherDost) // есть карта выше твоей пары
                                    {
                                        weak = true;
                                    }
                                }
                            }
                        }
                    }

                    // проверка что кикер не ниже валета
                    if (myCard[0].Dost != myCard[1].Dost)
                    {
                        if (myCard[0].Dost != parentDost && myCard[0].Dost != otherDost)
                        {
                            if (myCard[0].Dost < 9)
                                weak = true;
                        }
                        if (myCard[1].Dost != parentDost && myCard[1].Dost != otherDost)
                        {
                            if (myCard[1].Dost < 9)
                                weak = true;
                        }
                    }

                    if (weak)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Две пары (слабые)";
                        cmb.Combinaton = FCombinaton.WeakTwoPair;
                    }
                    else
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Две пары (как топ пара)";
                        cmb.Combinaton = FCombinaton.TwoPairLikePair;
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает пару
        /// </summary>
        private static FlopCombination GetPair(List<Card> myCard, List<Card> flopCard)
        {
            List<Card> card = new List<Card>(flopCard);
            card.Add(myCard[0]);
            card.Add(myCard[1]);

            FlopCombination cmb = null;

            var dict = new Dictionary<int, int>();

            for (int i = 0; i < card.Count; i++)
            {
                var dost = card[i].Dost;
                if (dict.ContainsKey(dost))
                {
                    dict[dost] += 1;
                }
                else
                {
                    dict.Add(dost, 1);
                }
            }

            foreach (KeyValuePair<int, int> pair in dict)
            {
                if (pair.Value == 2)
                {
                    bool fFirstIsPair = false;
                    bool fsecondIsPair = false;

                    fFirstIsPair = myCard[0].Dost == pair.Key;
                    fsecondIsPair = myCard[1].Dost == pair.Key;

                    if (!fFirstIsPair && !fsecondIsPair)
                    {
                        cmb = new FlopCombination();
                        cmb.Name = "Пара (слабая)";
                        cmb.Combinaton = FCombinaton.WeakPair;
                    }
                    else if (fFirstIsPair && fsecondIsPair)
                    {
                        // Это оверпара или слабая пара
                        bool weak = false;
                        foreach (Card card1 in flopCard)
                        {
                            if (card1.Dost > myCard[0].Dost)
                            {
                                weak = true;
                            }
                        }

                        if (!weak)
                        {
                            // Если оверпара младше семерок
                            if (myCard[0].Dost < 5)
                                weak = true;
                        }

                        if (weak)
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Пара (слабая)";
                            cmb.Combinaton = FCombinaton.WeakPair;
                        }
                        else
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Оверпара";
                            cmb.Combinaton = FCombinaton.OverPair;
                        }
                    }
                    else
                    {
                        int dost = fFirstIsPair ? myCard[0].Dost : myCard[1].Dost;

                        bool weak = false;
                        foreach (Card card1 in flopCard)
                        {
                            if (card1.Dost > dost)
                            {
                                weak = true;
                            }
                        }

                        if (!weak)
                        {
                            // проверка что пара не ниже десятки
                            if (fFirstIsPair && myCard[1].Dost < 8)
                                weak = true;
                            if (fsecondIsPair && myCard[0].Dost < 8)
                                weak = true;
                        }
                        if (!weak)
                        {
                            // проверка что кикер не ниже валета
                            int dostKicker = fFirstIsPair ? myCard[1].Dost : myCard[0].Dost;
                            if (dostKicker < 9)
                            {
                                weak = true;
                            }
                        }


                        if (weak)
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Пара (слабая)";
                            cmb.Combinaton = FCombinaton.WeakPair;
                        }
                        else
                        {
                            cmb = new FlopCombination();
                            cmb.Name = "Топ пара";
                            cmb.Combinaton = FCombinaton.TopPair;
                        }
                    }
                }
            }

            return cmb;
        }

        /// <summary>
        /// Возвращает оверкарты
        /// </summary>
        private static FlopCombination GetOverCard(List<Card> myCard, List<Card> flopCard)
        {
            FlopCombination cmb = null;

            int maxDost = 0;
            foreach (Card card in flopCard)
            {
                if (card.Dost > maxDost)
                    maxDost = card.Dost;
            }

            if (myCard[0].Dost > maxDost && myCard[1].Dost > maxDost)
            {
                cmb = new FlopCombination();
                cmb.Name = "Оверкарты";
                cmb.Combinaton = FCombinaton.OverCards;
            }
            else if (myCard[0].Dost > maxDost || myCard[1].Dost > maxDost)
            {
                cmb = new FlopCombination();
                cmb.Name = "Оверкарта";
                cmb.Combinaton = FCombinaton.OverCard;
            }

            return cmb;
        }

        #endregion


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Name);
            foreach (FCombinaton fCombinaton in Dro)
            {
                builder.AppendLine(fCombinaton.ToString());
            }
            builder.AppendLine("Lev: " + GetLevel(this));

            return builder.ToString();
        }

        public static void Test()
        {
            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0,12));
                myCard.Add(new Card(0, 11));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(1, 11));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 11));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ каре");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 12));
                myCard.Add(new Card(1, 12));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(1, 11));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 11));
                flopCard.Add(new Card(0, 11));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабое каре");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 12));
                myCard.Add(new Card(0, 11));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 10));
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(3, 12));
                flopCard.Add(new Card(2, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит флэш");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 6));
                myCard.Add(new Card(0, 11));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 10));
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(0, 2));
                flopCard.Add(new Card(0, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ не стрит флэш");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 3));
                myCard.Add(new Card(1, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(1, 11));
                flopCard.Add(new Card(3, 12));
                flopCard.Add(new Card(2, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ фулхаус");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 3));
                myCard.Add(new Card(1, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(1, 11));
                flopCard.Add(new Card(3, 11));
                flopCard.Add(new Card(2, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый фулхаус");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 9));
                myCard.Add(new Card(0, 10));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(0, 2));
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(0, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ флэш");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 12));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(0, 2));
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(0, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ флэш");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 2));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(0, 12));
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(0, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый флэш");
                Trace.WriteLine(cmb.ToString());
            }

            ////////////////////////
            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(0, 3));
                myCard.Add(new Card(0, 10));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(0, 2));
                flopCard.Add(new Card(3, 3));
                flopCard.Add(new Card(0, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ флэш дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 11));
                myCard.Add(new Card(0, 12));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(0, 2));
                flopCard.Add(new Card(0, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ флэш дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 2));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(0, 12));
                flopCard.Add(new Card(0, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый флэш дро");
                Trace.WriteLine(cmb.ToString());
            }

            /////////////////////////////////////////////
            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 2));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 4));
                flopCard.Add(new Card(2, 5));
                flopCard.Add(new Card(3, 6));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 7));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(2, 10));
                flopCard.Add(new Card(3, 6));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 2));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(2, 4));
                flopCard.Add(new Card(3, 5));
                flopCard.Add(new Card(0, 6));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 9));
                flopCard.Add(new Card(0, 6));
                flopCard.Add(new Card(1, 5));
                flopCard.Add(new Card(1, 7));
                flopCard.Add(new Card(0, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый стрит");
                Trace.WriteLine(cmb.ToString());
            }


            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 7));
                myCard.Add(new Card(0, 7));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(1, 10));
                flopCard.Add(new Card(1, 9));
                flopCard.Add(new Card(2, 11));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 8));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 9));
                flopCard.Add(new Card(0, 12));
                flopCard.Add(new Card(1, 11));
                flopCard.Add(new Card(1, 10));
                flopCard.Add(new Card(2, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 12));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 0));
                flopCard.Add(new Card(0, 1));
                flopCard.Add(new Card(1, 2));
                flopCard.Add(new Card(1, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый стрит");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 12));
                myCard.Add(new Card(0, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(2, 0));
                flopCard.Add(new Card(0, 1));
                flopCard.Add(new Card(1, 2));
                flopCard.Add(new Card(1, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит");
                Trace.WriteLine(cmb.ToString());
            }

            ///////////////////////////////
            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 2));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 4));
                flopCard.Add(new Card(2, 5));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 7));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(2, 10));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит дро");
                Trace.WriteLine(cmb.ToString());
            }


            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 7));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 5));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 7));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 10));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 5));
                flopCard.Add(new Card(3, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ стрит дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 5));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 10));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 9));
                flopCard.Add(new Card(3, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабое стрит дро");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 5));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 7));
                flopCard.Add(new Card(2, 12));
                flopCard.Add(new Card(3, 9));
                flopCard.Add(new Card(3, 4));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ гат шот");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 12));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 1));
                flopCard.Add(new Card(2, 2));
                flopCard.Add(new Card(3, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ слабый гат шот");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 12));
                myCard.Add(new Card(0, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 1));
                flopCard.Add(new Card(2, 2));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ гат шот");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 5));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 12));
                flopCard.Add(new Card(2, 12));
                flopCard.Add(new Card(3, 9));
                flopCard.Add(new Card(3, 10));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ гат шот (потому что с десятки)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 4));
                myCard.Add(new Card(0, 7));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(2, 11));
                flopCard.Add(new Card(3, 8));
                flopCard.Add(new Card(3, 9));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ гат шот (слабый)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 4));
                myCard.Add(new Card(0, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(2, 12));
                flopCard.Add(new Card(3, 8));
                flopCard.Add(new Card(3, 9));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ гат шот (слабый)");
                Trace.WriteLine(cmb.ToString());
            }
            ////////////////////////

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 3));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(2, 12));
                flopCard.Add(new Card(3, 8));
                flopCard.Add(new Card(3, 3));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ трипс");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 8));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 11));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ трипс (с одной картой)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 8));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ трипс (слабый)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 3));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(2, 9));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 9));
                myCard.Add(new Card(0, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 3));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары (как топ пара)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 10));
                myCard.Add(new Card(2, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары (как топ пара)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 1));
                myCard.Add(new Card(2, 1));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары (слабые)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 0));
                myCard.Add(new Card(2, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары (слабые)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 10));
                myCard.Add(new Card(2, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 8));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ две пары (слабые)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 10));
                myCard.Add(new Card(2, 9));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 2));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ топ пара");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 10));
                myCard.Add(new Card(2, 12));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 9));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ топ пара");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 4));
                myCard.Add(new Card(2, 4));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 1));
                flopCard.Add(new Card(3, 2));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ пара (слабая)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 12));
                myCard.Add(new Card(2, 5));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 1));
                flopCard.Add(new Card(3, 12));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ пара (слабая)");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 11));
                myCard.Add(new Card(2, 11));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 9));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ оверпара");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 11));
                myCard.Add(new Card(2, 12));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 7));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ оверкарты");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 11));
                myCard.Add(new Card(2, 1));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 7));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ оверкарта");
                Trace.WriteLine(cmb.ToString());
            }

            {
                List<Card> myCard = new List<Card>();
                myCard.Add(new Card(1, 2));
                myCard.Add(new Card(2, 1));

                List<Card> flopCard = new List<Card>();
                flopCard.Add(new Card(0, 0));
                flopCard.Add(new Card(2, 8));
                flopCard.Add(new Card(3, 7));

                FlopCombination cmb = GetCombination(myCard, flopCard);
                Trace.WriteLine("+++ ничего");
                Trace.WriteLine(cmb.ToString());
            }
        }
    }
}
