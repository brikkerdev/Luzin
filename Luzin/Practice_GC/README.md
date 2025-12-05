Задача 1: Stack + Dictionary для отслеживания истории
Создать обобщённый класс VersionedStack:
Внутри Stack хранит элементы, в Dictionary<int, T> — версии элементов.
Реализовать методы Push(T item), Pop(), GetVersion(int version).
Продемонстрировать на примере текстовых изменений документа.

Задача 2: Generic фильтр с делегатами
Создать обобщённый метод FilterCollection:
Принимает IEnumerable и Func<T, bool> условие
Возвращает коллекцию элементов, удовлетворяющих условию
Проверить на списке Product с фильтром по цене и списке Student с фильтром по среднему баллу.

Задача 3: SortedList<TKey, TValue> и SortedDictionary<TKey, TValue>
Создать SortedList<int, string> и SortedDictionary<int, string> для пар Id → Name. 
Добавить элементы в случайном порядке и вывести их. Измерить время добавления 10000
элементов в каждую коллекцию и сравнить скорость.

Инструкция к запуску:
1) Скопировать код из Task в Program.cs
2) Запустить программу