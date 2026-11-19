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
