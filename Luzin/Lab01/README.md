# Лабораторная работа 1: JSON-сериализация и работа с файлами в C#

## Цели работы
1. Использовать JSON-атрибуты для управления сериализацией/десериализацией
2. Применять `IDisposable` для корректного освобождения ресурсов
3. Выполнять чтение/запись файлов в UTF-8
4. Сохранять/загружать объекты в/из JSON-файлов
5. Понимать отличие `Dispose()` от финализатора (`~Class()`)

## Структура работы
- `Person.cs` — модель данных + атрибуты JSON + валидация
- `PersonSerializer.cs` — сериализация, файловые операции, логирование ошибок, потокобезопасность
- `FileResourceManager.cs` — управление `FileStream/StreamWriter/StreamReader` и `IDisposable`
- `TestPersonSerializer.cs` — консольные тесты

## Задание 1: Класс `Person`
`Person` содержит обычные свойства и свойства/поля с атрибутами JSON.

### Поля и свойства
- `FirstName`, `LastName`, `Age` — обычные свойства
- `Password` — не должен попадать в JSON (`[JsonIgnore]`)
- `Id` — сериализуется как `personId` (`[JsonPropertyName("personId")]`)
- `_birthDate` — приватное поле, включаемое в JSON (`[JsonInclude]`)
- `Email` — свойство с валидацией (не пустое, содержит `@`)
- `PhoneNumber` — сериализуется как `phone` (`[JsonPropertyName("phone")]`)
- `FullName` — вычисляемое свойство (в JSON не нужно)
- `IsAdult` — вычисляемое свойство (в JSON не нужно)

### Особенности
- Реализован `IDisposable`
- Присутствует финализатор `~Person()`
- Валидация в сеттерах
- Текстовые операции в проекте используют UTF-8 (в сериализаторе)

## Задание 2: `PersonSerializer`
Класс предоставляет методы:
1. `SerializeToJson(Person person)` — сериализация в строку JSON
2. `DeserializeFromJson(string json)` — десериализация из строки
3. `SaveToFile(Person person, string filePath)` — синхронное сохранение
4. `LoadFromFile(string filePath)` — синхронная загрузка
5. `SaveToFileAsync(Person person, string filePath)` — асинхронное сохранение
6. `LoadFromFileAsync(string filePath)` — асинхронная загрузка
7. `SaveListToFile(List<Person> people, string filePath)` — экспорт списка
8. `LoadListFromFile(string filePath)` — импорт списка

## Задание 3: `FileResourceManager`
`FileResourceManager` демонстрирует работу с файловыми ресурсами:
- открытие файла на чтение/запись через `FileStream`;
- использование `StreamWriter` и `StreamReader`;
- методы `WriteLine`, `ReadAllText`, `AppendText`, `GetFileInfo`;
- корректное освобождение ресурсов через `IDisposable` + финализатор.

## Задание 4: Тестирование

### Сценарии
1. Сериализация/десериализация `Person`
2. Сохранение/загрузка из файла
3. Работа со списками `List<Person>`
4. Обработка ошибок (несуществующий файл, некорректный JSON)
5. Асинхронные сохранение/загрузка

### Инструкция к запуску тестов

```bash
dotnet test