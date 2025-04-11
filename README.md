=======================================
# Advertising Platforms API
=======================================

Это веб-сервис для управления и поиска рекламных площадок по географическим локациям.

---------------------------------------
Запуск проекта
---------------------------------------

1. Убедитесь, что у вас установлен .NET 8 SDK
2. Запустите приложение


---------------------------------------
# Использование через Swagger UI
---------------------------------------

После запуска приложения у Вас в браузере откроется Swagger

Вы увидите интерфейс Swagger с документацией API:

---------------------------------------
1. Загрузка рекламных площадок
---------------------------------------

Эндпоинт: POST /api/platform/upload

- Нажмите на эндпоинт POST /api/platform/upload
- Нажмите кнопку "Try it out"
- В поле "file" нажмите "Choose file" и выберите текстовый файл с данными
- Нажмите "Execute"

Формат файла:

  Название площадки1:/локация1,/локация2
  Название площадки2:/локация3,/локация4

Пример:

  Яндекс.Директ:/ru
  Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
  Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl

---------------------------------------
2. Поиск рекламных площадок
---------------------------------------

Эндпоинт: GET /api/platform/search

- Нажмите на эндпоинт GET /api/platform/search
- Нажмите кнопку "Try it out"
- Введите параметр:

  location - локация для поиска (например "/ru/svrd/revda")

- Нажмите "Execute"

Пример ответа:

[
  {
    "name": "Яндекс.Директ",
    "locations": ["/ru"]
  },
  {
    "name": "Ревдинский рабочий",
    "locations": ["/ru/svrd/revda", "/ru/svrd/pervik"]
  }
]

---------------------------------------
Примеры использования
---------------------------------------

- Загрузите файл с данными через Swagger UI
- Выполните поиск для разных локаций:

  /ru                 - только глобальные площадки
  /ru/msk             - площадки для Москвы
  /ru/svrd/revda      - площадки для конкретного города

---------------------------------------
Особенности
---------------------------------------

- Данные хранятся в оперативной памяти (при перезапуске сервиса данные сбрасываются)
- Поддерживаются вложенные локации (дочерние локации наследуют площадки родительских)
- Максимальный размер загружаемого файла не ограничен

---------------------------------------
Тестирование
---------------------------------------

Для тестирования вы можете использовать пример файла из раздела "Формат файла" выше
или создать свой собственный.
