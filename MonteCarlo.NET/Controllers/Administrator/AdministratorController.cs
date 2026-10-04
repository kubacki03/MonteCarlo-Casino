using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;


namespace MonteCarlo.NET.Controllers.Administrator
{
    public class AdministratorController : Controller
    {
        private readonly CasinoContext _context;
        public AdministratorController(CasinoContext context)
        {
            this._context = context;
        }
		[Authorize(Roles = "Administrator")]
        public IActionResult Users(string? sortOption, string? searchText)
        {
            string[] userSortOptions = new string[]
            {
                "Domyślnie",
                "Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
                "Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
                "Level rosnąco",
                "Level malejąco",
                "Saldo rosnąco",
                "Saldo malejąco",
				"Data końca banu rosnąco",
                "Data końca banu malejąco"
            };
            ViewBag.userSortOptions = userSortOptions;
			ViewBag.SelectedOption = sortOption;

			IQueryable<UserAccount> usersQuery = _context.UserAccounts;

			
			if (!string.IsNullOrEmpty(searchText))
			{
				usersQuery = usersQuery.Where(u => u.UserName.Contains(searchText) || u.FirstName.Contains(searchText) || u.LastName.Contains(searchText) || u.Balance.ToString().Contains(searchText) || u.Level.ToString().Contains(searchText) || u.LockoutEnd.ToString().Contains(searchText));
			}

			
			switch (sortOption)
            {
                case "Nazwa użytkownika (A-Z)":
					usersQuery = usersQuery.OrderBy(nu => nu.UserName);
                    break;
                case "Nazwa użytkownika (Z-A)":
					usersQuery = usersQuery.OrderByDescending(nu => nu.UserName);
                    break;
                case "Imie (A-Z)":
					usersQuery = usersQuery.OrderBy(nu => nu.FirstName);
                    break;
                case "Imie (Z-A)":
					usersQuery = usersQuery.OrderByDescending(nu => nu.FirstName);
                    break;
                case "Nazwisko (A-Z)":
					usersQuery = usersQuery.OrderBy(nu => nu.LastName);
                    break;
                case "Nazwisko (Z-A)":
					usersQuery = usersQuery.OrderByDescending(nu => nu.LastName);
                    break;
                case "Level rosnąco":
					usersQuery = usersQuery.OrderBy(nu => nu.Level);
                    break;
                case "Level malejąco":
					usersQuery = usersQuery.OrderByDescending(nu => nu.Level);
                    break;
                case "Saldo rosnąco":
					usersQuery = usersQuery.OrderBy(nu => nu.Balance);
                    break;
                case "Saldo malejąco":
					usersQuery = usersQuery.OrderByDescending(nu => nu.Balance);
                    break;
				case "Data końca banu rosnąco":
                    usersQuery = usersQuery.OrderBy(nu => nu.LockoutEnd);
                    break;
				case "Data końca banu malejąco":
                    usersQuery = usersQuery.OrderByDescending(nu => nu.LockoutEnd);
                    break;
                default:
                    break;
			}

			
			List<UserAccount> users = usersQuery.ToList();

			return View(users);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult SelectUserForBan(string userId)
		{
			HttpContext.Session.Remove("IdUzytkownika");
			HttpContext.Session.SetString("IdUzytkownika", userId);

            return RedirectToAction("BanForm"); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult BanForm(BanFormViewModel? banForm)
		{

			var userId = HttpContext.Session.GetString("IdUzytkownika");
			var user = _context.UserAccounts.FirstOrDefault(u => u.Id == userId);

			
			if (user != null)
			{
				if (user.LockoutEnabled)
				{
                    
                    if (ModelState.IsValid)
					{
                        Ban ban = new Ban()
                        {
                            Date = banForm.Date,
                            DurationDays = banForm.DurationDays,
                            Reason = banForm.Reason,
                            UserAccountId = user.Id
                        };
                        _context.Bans.Add(ban);
						user.LockoutEnabled = false;
						user.LockoutEnd = ban.Date.AddDays(ban.DurationDays);

						_context.Update(user);
						_context.SaveChanges();
						return RedirectToAction("Users");
					}
					return View(banForm);
				}
				else
				{
					user.LockoutEnabled = true;
					user.LockoutEnd = null;
					_context.Update(user);
					_context.SaveChanges();
				}
			}


			return RedirectToAction("Users");
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Ban(string? sortOption, string? searchText)
		{
			string[] banSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
				"Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
				"Data rosnąco",
				"Data malejąco",
				"Liczba dni rosnąco",
				"Liczba dni malejąco",
				"Przyczyna (A-Z)",
				"Przyczyna (Z-A)"

			};
			ViewBag.banSortOptions = banSortOptions;
			ViewBag.SelectedBanOption = sortOption;

			IQueryable<UserBan> bansQuery = _context.UserAccounts
				.Join(_context.Bans,
				u => u.Id,
				b => b.UserAccountId,
				(u, b) => new
				{
					u.UserName,
					u.FirstName,
					u.LastName,
					b.Date,
					b.DurationDays,
					b.Reason
				}).Select(ub => new UserBan
				{
					UserName = ub.UserName,
					FirstName = ub.FirstName,
					LastName = ub.LastName,
					Date = ub.Date,
					DurationDays = ub.DurationDays,
					Reason = ub.Reason
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				bansQuery = bansQuery.Where(ub => ub.UserName.Contains(searchText) || ub.FirstName.Contains(searchText) || ub.LastName.Contains(searchText) || ub.Date.ToString().Contains(searchText) || ub.DurationDays.ToString().Contains(searchText) || ub.Reason.Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa użytkownika (A-Z)":
					bansQuery = bansQuery.OrderBy(ub => ub.UserName);
					break;
				case "Nazwa użytkownika (Z-A)":
					bansQuery = bansQuery.OrderByDescending(ub => ub.UserName);
					break;
				case "Imie (A-Z)":
					bansQuery = bansQuery.OrderBy(ub => ub.FirstName);
					break;
				case "Imie (Z-A)":
					bansQuery = bansQuery.OrderByDescending(ub => ub.FirstName);
					break;
				case "Nazwisko (A-Z)":
					bansQuery = bansQuery.OrderBy(ub => ub.LastName);
					break;
				case "Nazwisko (Z-A)":
					bansQuery = bansQuery.OrderByDescending(ub => ub.LastName);
					break;
				case "Data rosnąco":
					bansQuery = bansQuery.OrderBy(ub => ub.Date);
					break;
				case "Data malejąco":
					bansQuery = bansQuery.OrderByDescending(ub => ub.Date);
					break;
				case "Liczba dni rosnąco":
					bansQuery = bansQuery.OrderBy(ub => ub.DurationDays);
					break;
				case "Liczba dni malejąco":
					bansQuery = bansQuery.OrderByDescending(ub => ub.DurationDays);
					break;
				case "Przyczyna (A-Z)":
					bansQuery = bansQuery.OrderBy(ub => ub.Reason);
					break;
				case "Przyczyna (Z-A)":
					bansQuery = bansQuery.OrderByDescending(ub => ub.Reason);
					break;
				default:
					break;
			}

			List<UserBan> rows = bansQuery.ToList();

			return View(rows);
		}



       
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> LiveChatAdmin()
        {
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult Limits(string? sortOption, string? searchText) 
		{
			string[] limitSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
				"Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
				"Data rosnąco",
				"Data malejąco",
				"Kwota rosnąco",
				"Kwota malejąco"
			};
			ViewBag.limitSortOptions = limitSortOptions;
			ViewBag.SelectedLimitOption = sortOption;

			IQueryable<UserLimit> limitsQuery = _context.UserAccounts
				.Join(_context.Limits,
				u => u.Id,
				l => l.UserAccountId,
				(u, l) => new
				{
					u.UserName,
					u.FirstName,
					u.LastName,
					l.Date,
					l.Amount
				}).Select(ul => new UserLimit
				{
					UserName = ul.UserName,
					LastName = ul.LastName,
					FirstName = ul.FirstName,
					Date = ul.Date,
					Amount = (float)ul.Amount
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				limitsQuery = limitsQuery.Where(ul => ul.UserName.Contains(searchText) || ul.FirstName.Contains(searchText) || ul.LastName.Contains(searchText) || ul.Date.ToString().Contains(searchText) || ul.Amount.ToString().Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa użytkownika (A-Z)":
					limitsQuery = limitsQuery.OrderBy(ul => ul.UserName);
					break;
				case "Nazwa użytkownika (Z-A)":
					limitsQuery = limitsQuery.OrderByDescending(ul => ul.UserName);
					break;
				case "Imie (A-Z)":
					limitsQuery = limitsQuery.OrderBy(ul => ul.FirstName);
					break;
				case "Imie (Z-A)":
					limitsQuery = limitsQuery.OrderByDescending(ul => ul.FirstName);
					break;
				case "Nazwisko (A-Z)":
					limitsQuery = limitsQuery.OrderBy(ul => ul.LastName);
					break;
				case "Nazwisko (Z-A)":
					limitsQuery = limitsQuery.OrderByDescending(ul => ul.LastName);
					break;
				case "Data rosnąco":
					limitsQuery = limitsQuery.OrderBy(ul => ul.Date);
					break;
				case "Data malejąco":
					limitsQuery = limitsQuery.OrderByDescending(ul => ul.Date);
					break;
				case "Kwota rosnąco":
					limitsQuery = limitsQuery.OrderBy(ul => ul.Amount);
					break;
				case "Kwota malejąco":
					limitsQuery = limitsQuery.OrderByDescending(ul => ul.Amount);
					break;
				default:
					break;
			}

			List<UserLimit> rows = limitsQuery.ToList();

			return View(rows); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Games(string? sortOption, string? searchText)
		{
			string[] gameSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa gry (A-Z)",
				"Nazwa gry (Z-A)",
				"Minimalna stawka rosnąco",
				"Minimalna stawka malejąco"
			};
			ViewBag.gameSortOptions = gameSortOptions;
			ViewBag.SelectedGameOption = sortOption;
			IQueryable<Game> gamesQuery = _context.Games;

			if (!string.IsNullOrEmpty(searchText))
			{
				gamesQuery = gamesQuery.Where(g => g.Name.Contains(searchText) || g.MinStake.ToString().Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa gry (A-Z)":
					gamesQuery = gamesQuery.OrderBy(g => g.Name);
					break;
				case "Nazwa gry (Z-A)":
					gamesQuery = gamesQuery.OrderByDescending(g => g.Name);
					break;
				case "Minimalna stawka rosnąco":
					gamesQuery = gamesQuery.OrderBy(g => g.MinStake);
					break;
				case "Minimalna stawka malejąco":
					gamesQuery = gamesQuery.OrderByDescending(g => g.MinStake);
					break;
				default:
					break;
			}

			List<Game> rows = gamesQuery.ToList();

			return View(rows); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult UpdateMinStake(int gameId, int newStake)
		{
			var game = _context.Games.Find(gameId);
			if (game != null)
			{
				game.MinStake = newStake;
				_context.SaveChanges();
			}
			return RedirectToAction("Games");
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Transactions(string? sortOption, string? searchText)
		{
			string[] transactionSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
				"Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
				"Data rosnąco",
				"Data malejąco",
				"Kwota rosnąco",
				"Kwota malejąco",
				"Typ (A-Z)",
				"Typ (Z-A)"
			};
			ViewBag.transactionSortOptions = transactionSortOptions;
			ViewBag.SelectedTransactionOption = sortOption;

			IQueryable<UserTransaction> transactionsQuery = _context.UserAccounts
				.Join(_context.Transactions,
				u => u.Id,
				t => t.UserAccountId,
				(u, t) => new
				{
					u.UserName,
					u.LastName,
					u.FirstName,
					t.Date,
					t.Amount,
					t.Type
				}).Select(ut => new UserTransaction
				{
					UserName = ut.UserName,
					LastName = ut.LastName,
					FirstName = ut.FirstName,
					Date = ut.Date,
					Amount = (float)ut.Amount,
					Type = ut.Type
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				transactionsQuery = transactionsQuery.Where(ut => ut.UserName.Contains(searchText) || ut.FirstName.Contains(searchText) || ut.LastName.Contains(searchText) || ut.Date.ToString().Contains(searchText) || ut.Amount.ToString().Contains(searchText) || ut.Type.Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa użytkownika (A-Z)":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.UserName);
					break;
				case "Nazwa użytkownika (Z-A)":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.UserName);
					break;
				case "Imie (A-Z)":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.FirstName);
					break;
				case "Imie (Z-A)":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.FirstName);
					break;
				case "Nazwisko (A-Z)":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.LastName);
					break;
				case "Nazwisko (Z-A)":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.LastName);
					break;
				case "Data rosnąco":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.Date);
					break;
				case "Data malejąco":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.Date);
					break;
				case "Kwota rosnąco":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.Amount);
					break;
				case "Kwota malejąco":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.Amount);
					break;
				case "Typ (A-Z)":
					transactionsQuery = transactionsQuery.OrderBy(ut => ut.Type);
					break;
				case "Typ (Z-A)":
					transactionsQuery = transactionsQuery.OrderByDescending(ut => ut.Type);
					break;
				default:
					break;
			}

			List<UserTransaction> rows = transactionsQuery.ToList();

			return View(rows);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult GameAccounts(string? sortOption, string? searchText)
		{
			string[] gameAccountSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
				"Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
				"Wygrana rosnąco",
				"Wygrana malejąco",
				"Postawienie rosnąco",
				"Postawienie malejąco",
				"Czas rosnąco",
				"Czas malejąco",
				"Nazwa gry (A-Z)",
				"Nazwa gry (Z-A)"
			};
			ViewBag.gameAccountSortOptions = gameAccountSortOptions;
			ViewBag.SelectedGameAccountOption = sortOption;

			IQueryable<UserGameAccount> gameAccountsQuery = _context.UserAccounts
				.Join(_context.GameAccounts,
				u => u.Id,
				gk => gk.UserAccountId,
				(u, gk) => new
				{
					u.UserName,
					u.FirstName,
					u.LastName,
					gk.AmountWon,
					gk.AmountStaked,
					gk.PlayedAt,
					gk.GameId
				}).Join(_context.Games,
				ugk => ugk.GameId,
				g => g.GameId,
				(ugk, g) => new
				{
					ugk.UserName,
					ugk.FirstName,
					ugk.LastName,
					ugk.AmountWon,
					ugk.AmountStaked,
					ugk.PlayedAt,
					g.Name
				}).Select(x => new UserGameAccount
				{
					UserName = x.UserName,
					LastName = x.LastName,
					FirstName = x.FirstName,
					AmountWon = (float)x.AmountWon,
					AmountStaked = (float)x.AmountStaked,
					PlayedAt = (DateTime)x.PlayedAt,
					GameName = x.Name
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				gameAccountsQuery = gameAccountsQuery.Where(gk => gk.UserName.Contains(searchText) || gk.FirstName.Contains(searchText) || gk.LastName.Contains(searchText) || gk.AmountWon.ToString().Contains(searchText) || gk.AmountStaked.ToString().Contains(searchText) || gk.PlayedAt.ToString().Contains(searchText) || gk.GameName.Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa użytkownika (A-Z)":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.UserName);
					break;
				case "Nazwa użytkownika (Z-A)":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.UserName);
					break;
				case "Imie (A-Z)":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.FirstName);
					break;
				case "Imie (Z-A)":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.FirstName);
					break;
				case "Nazwisko (A-Z)":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.LastName);
					break;
				case "Nazwisko (Z-A)":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.LastName);
					break;
				case "Wygrana rosnąco":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.AmountWon);
					break;
				case "Wygrana malejąco":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.AmountWon);
					break;
				case "Postawienie rosnąco":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.AmountStaked);
					break;
				case "Postawienie malejąco":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.AmountStaked);
					break;
				case "Czas rosnąco":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.PlayedAt);
					break;
				case "Czas malejąco":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.PlayedAt);
					break;
				case "Nazwa gry (A-Z)":
					gameAccountsQuery = gameAccountsQuery.OrderBy(gk => gk.GameName);
					break;
				case "Nazwa gry (Z-A)":
					gameAccountsQuery = gameAccountsQuery.OrderByDescending(gk => gk.GameName);
					break;
				default:
					break;
			}

			List<UserGameAccount> rows = gameAccountsQuery.ToList();
			return View(rows);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Reports(string? sortOption, string? searchText)
		{
			string[] reportSortOptions = new string[]
			{
				"Domyślnie",
				"Nazwa użytkownika (A-Z)",
				"Nazwa użytkownika (Z-A)",
				"Imie (A-Z)",
				"Imie (Z-A)",
				"Nazwisko (A-Z)",
				"Nazwisko (Z-A)",
				"Data rosnąco",
				"Data malejąco",
				"Status (A-Z)",
				"Status (Z-A)",
				"Tytuł (A-Z)",
				"Tytuł (Z-A)"
			};
			ViewBag.reportSortOptions = reportSortOptions;
			ViewBag.SelectedReportOption = sortOption;

			IQueryable<UserReport> reportsQuery = _context.UserAccounts
				.Join(_context.Reports,
				u => u.Id,
				z => z.UserAccountId,
				(u, z) => new
				{
					u.UserName,
					u.LastName,
					u.FirstName,
					z.Title,
					z.Status,
					z.Date,
					z.ReportId
				}).Select(uz => new UserReport
				{
					ReportId = uz.ReportId,
					UserName = uz.UserName,
					LastName = uz.LastName,
					FirstName = uz.FirstName,
					Status = uz.Status,
					Date = uz.Date,
					Title = uz.Title
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				reportsQuery = reportsQuery.Where(uz => uz.UserName.Contains(searchText) || uz.FirstName.Contains(searchText) || uz.LastName.Contains(searchText) || uz.Status.Contains(searchText) || uz.Title.Contains(searchText) || uz.Date.ToString().Contains(searchText));
			}

			switch (sortOption)
			{
				case "Nazwa użytkownika (A-Z)":
					reportsQuery = reportsQuery.OrderBy(uz => uz.UserName);
					break;
				case "Nazwa użytkownika (Z-A)":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.UserName);
					break;
				case "Imie (A-Z)":
					reportsQuery = reportsQuery.OrderBy(uz => uz.FirstName);
					break;
				case "Imie (Z-A)":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.FirstName);
					break;
				case "Nazwisko (A-Z)":
					reportsQuery = reportsQuery.OrderBy(uz => uz.LastName);
					break;
				case "Nazwisko (Z-A)":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.LastName);
					break;
				case "Data rosnąco":
					reportsQuery = reportsQuery.OrderBy(uz => uz.Date);
					break;
				case "Data malejąco":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.Date);
					break;
				case "Status (A-Z)":
					reportsQuery = reportsQuery.OrderBy(uz => uz.Status);
					break;
				case "Status (Z-A)":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.Status);
					break;
				case "Tytuł (A-Z)":
					reportsQuery = reportsQuery.OrderBy(uz => uz.Title);
					break;
				case "Tytuł (Z-A)":
					reportsQuery = reportsQuery.OrderByDescending(uz => uz.Title);
					break;
				default:
					break;
			}

			List<UserReport> rows = reportsQuery.ToList();

			return View(rows);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult ReportDetails(int reportId)
		{
			var report = _context.Reports.Find(reportId);
			if (report == null)
			{
				return NotFound();
			}
			return View(report);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult ChangeStatus(int reportId, string newStatus)
		{
			var report = _context.Reports.Find(reportId);
            if (report == null)
            {
                return NotFound();
            }

			report.Status = newStatus;
			_context.SaveChanges();
			return RedirectToAction("Reports");
		}


        [Authorize(Roles = "Administrator")]
        public IActionResult HealthCheck()
        {
          
            return View();
        }

    }
}
