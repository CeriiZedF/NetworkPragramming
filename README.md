# NetworkPragramming
1)Д.З. Реалізувати реакцію UI на перемикання стану сервера -
змінювати надпис та колір статусу, надпис (опціонально-колір)
кнопки запуску/зупинки серверу.
https://github.com/CeriiZedF/NetworkPragramming/blob/e7c4723c18343cf69fdee56066e3adecfa42c5e3/ServerWindow.xaml.cs#L177

2)Д.З. Реалізувати відображення статусу серверної відповіді
на клієнті при надсиланні повідомлення
- при успішному статусі: зелений фон + "надіслано"
- при помилці: червоний фон + "помилка"
 прибирати це повідомлення через 3 секунди після показу
https://github.com/CeriiZedF/NetworkPragramming/blob/40fb4670f1bd3f645b5ccf1ad326327c25d14d2b/ClientWindow.xaml.cs#L191

3)Д.З. Реалізувати відображення часу повідомлення на клієнті
у "розумному" стилі: якщо у межах поточної дати, то 
писати "сьогодні" та час. Якщо старші за день,
то ще й додавати дату. При зміні дати оновлювати відображення
Створити Гугл-акаунт, встановити двофакторну перевірку.
https://github.com/CeriiZedF/NetworkPragramming/blob/76a78203a9292bb031a0066b9c21cea201b70e80/ClientWindow.xaml.cs#L226

4)Реалізувати перевірку файлу конфігурації на правильну структуру (можливість парсингу JSON).
За наявності відхилень видавати повідомлення на кшталт "Файл конфігурації має неправильну
структуру або пошкоджений"
![FileJsonError](https://github.com/CeriiZedF/NetworkPragramming/assets/60105990/ba92bcf7-2ce2-4e6c-a312-73b07b5acb7a)

5)Д.З. Реалізувати надсилання електронного листа із 
HTML контентом, який включає код підтвердження пошти
(взяти довільний код), а також вкладення файлу
privacy.txt з політикою сайту (взяти довільну)
із зазначенням правильного MIME-типу
також додати файл privacy.doc з тим самим вмістом
![EmailBot](https://github.com/CeriiZedF/NetworkPragramming/assets/60105990/812f92d6-ad97-49f6-99cf-9ddc169b475a)

6) При правильному вводі коду підтвердження пошти
виконати SQL запит, який змінить значення ConfirmCode
на NULL, а також приховає "вікно" введення коду.
Перевірити, що повторному вході пошта вважається
підтвердженою
писати "сьогодні" та час. Якщо старші за день,
то ще й додавати дату. При зміні дати оновлювати відображення
Створити Гугл-акаунт, встановити двофакторну перевірку.
https://github.com/CeriiZedF/NetworkPragramming/blob/c13ec0ece5ea636795934729f36ec735a6af691e/AuthWindow.xaml.cs#L121

7)Д.З. Додати до вікна HttpWindow дві кнопки, які
 завантажують дані про курси валют в різних форматах (XML, JSON)
 https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange
 https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json
 та виводять їх у вигляді тексту на інтерфейс вікна.
 Додати скріншоти
 ![JsonText](https://github.com/CeriiZedF/NetworkPragramming/assets/60105990/30b6f11f-eaa1-49d3-af56-31a62df14d8e)
 
 ![XMLText](https://github.com/CeriiZedF/NetworkPragramming/assets/60105990/4a63a569-3617-45fd-9c92-dbc9185fc9ba)


8) Д.З. Реалізувати "перенесення" виділення елемента списку:
при виділенні іншого елемента знімати виділення з попереднього.
Видати повідомлення (MessageBox) про id ассета, що виділяється.
Повторити роботу з WPF Canvas (принаймні рисування ліній)



 
