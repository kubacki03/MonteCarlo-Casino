This document provides a brief description of the libraries used in this project.
## Demo
https://drive.google.com/file/d/10lQV5FzIoWe3IUnJnW6jPc_q2rL5V7pw/view?usp=drive_link

## Important
This is our group project for web development in ASP.NET Core. For this project, I was responsible for the following features:
1) Chatbot
2) Live chat with admin
3) Age verification at registration using image recognition of the user's ID
4) Scratch card games
5) Football betting system
6) Stripe payments integration
7) Payouts with mandatory confirmation via a code sent to the user's email
8) User statistics panel
9) Self-exclusion feature for users
10) Implementing a health check middleware to catch errors and forward them to the admin panel

## Ważne
Jest to nasz projekt grupowy w zakresie tworzenia aplikacji webowych w ASP.NET Core. W tym projekcie byłem odpowiedzialny za następujące funkcje:
1)Chatbot
2)Czat na żywo z administratorem
3)Weryfikacja wieku podczas rejestracji za pomocą rozpoznawania obrazu dokumentu tożsamości użytkownika
4)Gry typu scratch card
5)Zakłady piłkarskie
6)Integracja z płatnościami Stripe
7)Wypłaty z obowiązkową weryfikacją za pomocą kodu wysłanego na adres e-mail użytkownika
8)Panel statystyk użytkownika
9)Funkcja samowykluczenia z platformy przez użytkownika
10)Implementacja middleware do monitorowania stanu aplikacji, które przechwytuje błędy i przekazuje je do panelu administratora


## Backend Libraries

1. **Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.11**
   - Integrates ASP.NET Core Identity with Entity Framework Core to manage user authentication and authorization.

2. **FluentAssertions 8.0.0**
   - Provides an expressive syntax for unit testing, improving readability and clarity when writing assertions in tests.

3. **Microsoft.AspNetCore.Identity.UI 8.0.11**
   - Contains Razor Pages for managing identity-related UI components such as login, registration, and password management.

4. **Microsoft.AspNetCore.SignalR 1.1.0**
   - A library for building real-time web applications, enabling server-to-client communication over WebSockets or other protocols.

5. **Microsoft.EntityFrameworkCore 8.0.11**
   - An Object-Relational Mapper (ORM) for .NET, allowing developers to interact with databases using .NET objects.

6. **Microsoft.NET.Test.Sdk 17.8.0**
   - Infrastructure for running unit tests in .NET, including handling test execution and reporting results.

7. **Microsoft.VisualStudio.Web.CodeGeneration.Design 8.0.7**
   - Supports code generation for ASP.NET Core applications, such as generating controllers and views.

8. **Moq 4.20.72**
   - A mocking framework for .NET that enables the creation of mock objects for unit testing.

9. **Stripe.net 47.1.0**
   - The official .NET library for integrating Stripe payment processing services.

10. **Tesseract 5.2.0**
    - An OCR library for extracting text from images.

11. **xUnit 2.9.3**
    - A unit testing framework for .NET.
