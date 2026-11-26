# Premier Online UI tests

Потрібні .NET SDK 10 та Chromium для Playwright.

```sh
dotnet restore
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

Якщо PowerShell не встановлений, браузер можна встановити драйвером, який постачається з Playwright:

```sh
bin/Debug/net10.0/.playwright/node/darwin-arm64/node bin/Debug/net10.0/.playwright/package/cli.js install chromium
```

Запуск на prod:

```sh
TEST_ENV=prod ALLOW_PRODUCTION_TESTS=true dotnet test
```

Сценарій `RegistrationSucceedsWithValidData` має атрибут `Explicit`: prod відхиляє автоматичну реєстрацію через reCAPTCHA. Перевірки заповнення форми та різних паролів запускаються у звичайному прогоні. Для повної реєстрації потрібне середовище з тестовими ключами reCAPTCHA; після його налаштування цей сценарій можна запустити окремо через `--filter Name=RegistrationSucceedsWithValidData`.

Адреса та облікові дані беруться з `.env.prod` у корені проєкту:

```dotenv
BASE_URL=https://www.premieronline.com
TEST_USER_EMAIL=your-test-account@example.com
TEST_USER_PASSWORD=your-test-password
```

Змінні процесу мають пріоритет над `.env.prod`. Без облікових даних пропускається лише тест успішного входу; також підтримується `testcredentials.json` з полями `email` та `password`. Обидва файли з секретами ігноруються Git.

Для stage вкажіть його окремий хост у `Config/environments.json`, URL у `.env.stage` та запустіть `TEST_ENV=stage dotnet test`. Для local підтримуються localhost, 127.0.0.1 та ::1. Prod URL не можна використовувати під назвою stage або local.

## Allure report

Перед першим запуском встановіть Allure CLI:

```sh
brew install allure
allure --version
```

Альтернативно через Node.js:

```sh
npm install -g allure
allure --version
```

Після запуску тестів Allure зберігає результати в `allure-results` у каталозі тестової
збірки (для Debug — `bin/Debug/net10.0/allure-results`):

```sh
dotnet test
allure serve bin/Debug/net10.0/allure-results
```

У звіті для `RegistrationTests` відображаються окремі кроки відкриття форми, заповнення,
відправлення та перевірки результату з їхніми статусами й тривалістю. PNG-скріншоти
повної сторінки додаються до відповідних кроків після відкриття форми, заповнення та
успішної перевірки результату. Повідомлення валідації й успіху спочатку очікуються на
сторінці, а потім знімаються. Якщо тест падає, базовий клас автоматично додає скріншот
стану сторінки до teardown; помилка його створення записується в NUnit і не підміняє
початкову помилку тесту. Для окремого
генерування звіту без автоматичного відкриття браузера використовуйте:

```sh
allure generate bin/Debug/net10.0/allure-results -o allure-report --clean
allure open allure-report
```

Після нового запуску тестів повторіть `allure generate`: вже згенерований каталог
`allure-report` автоматично не оновлюється. Щоб переглянути скріншот, відкрийте
`Suites` → `RegistrationTests` → потрібний тест → розгорніть крок у `Test body`
та натисніть назву PNG-вкладення. Якщо відкрито старий звіт через `allure serve`,
запустіть його знову з актуального каталогу результатів.

Перевірка конфігурації без браузера та мережі:

```sh
dotnet test --filter TestCategory=Unit
```

Структура:

- `Components/` — локатори, перевірки готовності й дії шапки та форм.
- `Pages/` — відкриття сторінок і компонування компонентів.
- `Tests/` — сценарії та перевірки результатів.
- `Config/` — завантаження `.env` та перевірка середовища.
- `TestData/` — тестові дані й завантаження облікових даних.
