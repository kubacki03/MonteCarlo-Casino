# MonteCarlo Casino

An online casino built with **ASP.NET Core 8 (MVC + Razor Pages)**, Entity Framework Core and SQL Server. It runs on a virtual currency ("BrigmaCoins") and Stripe in **test mode** — it is a learning project, not a real gambling product.

> Group university project for web development in ASP.NET Core. The list below marks the parts I built myself.

**Demo video:** https://drive.google.com/file/d/10lQV5FzIoWe3IUnJnW6jPc_q2rL5V7pw/view?usp=drive_link

<!-- TODO: add screenshots / a GIF of the slots, the admin panel and the chatbot (e.g. docs/screenshots/*.png) -->

## Features

**Games:** slots, roulette, dice, horse races, two scratch card games, football betting.

**Account & responsible gaming:** registration with age verification, daily stake limits, self-exclusion, bans managed by admins, player levels and ranking.

**Payments:** deposits with Stripe Checkout, payouts confirmed with a one-time code sent by e-mail.

**Support:** rule-based FAQ chatbot, live chat with an admin (SignalR).

**Admin panel:** users, bans, limits, games, transactions, reports, live chat and a health dashboard.

### What I built (my part of the group project)

1. Chatbot (Jaro-Winkler similarity matching against an FAQ)
2. Live chat with an admin (SignalR)
3. Age verification at registration using OCR of the user's ID (Tesseract)
4. Scratch card games
5. Football betting system
6. Stripe payments integration
7. Payouts with mandatory confirmation via a code sent to the user's e-mail
8. User statistics panel
9. Self-exclusion feature
10. Health check middleware that catches exceptions and forwards them to the admin panel

## Technical notes

- **Game logic lives on the server.** `GameService` runs every play inside a serializable transaction: it validates the stake, balance and daily limit, applies the result, records the play and updates the player level. Games (`SlotService`, `ScratchCardService`, `FootballBetService`) only decide the outcome. The client never reports a result — the slots endpoint receives only the stake and returns the reels drawn by the server.
- **House edge:** three matching reels out of 9 symbols happen with probability 1/81, the prize is 70× the stake, so the theoretical RTP is ~86%.
- **Payout flow:** a random 4-digit code (cryptographic RNG) is e-mailed, expires after 10 minutes and allows 5 attempts. The e-mail also contains a signed, time-limited "this wasn't me" link that reports the payout and locks the account for 15 minutes.
- **Stripe:** the balance is credited only when the Checkout session is `paid`.
- **No secrets in the repository:** the admin account, Stripe key and SMTP credentials come from configuration (see below).
- **Tests:** xUnit, with game services tested against an in-memory SQLite database and a scripted `Random`. Run them with `dotnet test`.

## Tech stack

ASP.NET Core 8 · EF Core 8 (SQL Server) · ASP.NET Core Identity · SignalR · Stripe.net · Tesseract OCR · AspNetCore.HealthChecks (+ UI) · xUnit

## Getting started

Requirements: .NET 8 SDK, SQL Server LocalDB (or any SQL Server — change `ConnectionStrings:MonteCarloDB`), the `dotnet-ef` tool.

```bash
git clone <this repo>
cd MonteCarlo-Casino/MonteCarlo.NET

# configuration is stored in user secrets, never in the repo
dotnet user-secrets set "Admin:Email" "admin@example.com"
dotnet user-secrets set "Admin:Password" "<a strong password>"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."        # Stripe test key, needed for deposits
dotnet user-secrets set "Email:From" "you@gmail.com"            # optional, payout e-mails
dotnet user-secrets set "Email:Password" "<gmail app password>" # optional

dotnet ef database update --context CasinoContext
dotnet run
```

On startup the app creates the `Administrator` role and the admin account from `Admin:Email` / `Admin:Password`. Without SMTP settings the payout e-mail is not sent (a warning is logged). On other environments use environment variables, e.g. `Stripe__SecretKey`.

The age verification needs the Polish Tesseract model, which is included in `MonteCarlo.NET/tessdata`.

## Project structure

```
MonteCarlo.NET/
  Controllers/   MVC controllers (thin - they delegate to services)
  Services/      game logic (Games/), e-mail, ID image processing
  Models/        entities and view models
  Data/          EF Core context, admin seeder
  HealthCheck/   CPU / exception health checks and exception tracking middleware
  Areas/Identity Customised Identity pages (registration with age verification)
TestyMonteCarlo/ xUnit tests
```

## Known limitations

- Roulette, dice and horse races still compute part of the result in controllers/the browser and should be moved to server-side services like the slots.
- Monetary values are stored as `double`; `decimal` would be the correct type.
- The chatbot is a simple FAQ matcher, not a language model.
