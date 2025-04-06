using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;


namespace MonteCarlo.NET.Controllers.Administrator
{
    public class AdministratorController : Controller
    {
        private readonly KasynoContext _context;
        public AdministratorController(KasynoContext context)
        {
            this._context = context;
        }
		[Authorize(Roles = "Administrator")]
        public IActionResult Uzytkownik(string? opcjaSort, string? searchText)
        {
            string[] opcjeSortowaniaUzytkownika = new string[]
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
            ViewBag.opcjeSortowaniaUzytkownika = opcjeSortowaniaUzytkownika;
			ViewBag.SelectedOption = opcjaSort;

			IQueryable<KontoUzytkownika> uzytkownicyQuery = _context.KontoUzytkownika;

			
			if (!string.IsNullOrEmpty(searchText))
			{
				uzytkownicyQuery = uzytkownicyQuery.Where(u => u.UserName.Contains(searchText) || u.Imie.Contains(searchText) || u.Nazwisko.Contains(searchText) || u.Saldo.ToString().Contains(searchText) || u.Level.ToString().Contains(searchText) || u.LockoutEnd.ToString().Contains(searchText));
			}

			
			switch (opcjaSort)
            {
                case "Nazwa użytkownika (A-Z)":
					uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.UserName);
                    break;
                case "Nazwa użytkownika (Z-A)":
					uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.UserName);
                    break;
                case "Imie (A-Z)":
					uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.Imie);
                    break;
                case "Imie (Z-A)":
					uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.Imie);
                    break;
                case "Nazwisko (A-Z)":
					uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.Nazwisko);
                    break;
                case "Nazwisko (Z-A)":
					uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.Nazwisko);
                    break;
                case "Level rosnąco":
					uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.Level);
                    break;
                case "Level malejąco":
					uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.Level);
                    break;
                case "Saldo rosnąco":
					uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.Saldo);
                    break;
                case "Saldo malejąco":
					uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.Saldo);
                    break;
				case "Data końca banu rosnąco":
                    uzytkownicyQuery = uzytkownicyQuery.OrderBy(nu => nu.LockoutEnd);
                    break;
				case "Data końca banu malejąco":
                    uzytkownicyQuery = uzytkownicyQuery.OrderByDescending(nu => nu.LockoutEnd);
                    break;
                default:
                    break;
			}

			
			List<KontoUzytkownika> uzytkownicy = uzytkownicyQuery.ToList();

			return View(uzytkownicy);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult FormularzPrzekazanieId(string uzytkownikId)
		{
			HttpContext.Session.Remove("IdUzytkownika");
			HttpContext.Session.SetString("IdUzytkownika", uzytkownikId);

            return RedirectToAction("FormularzBan"); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult FormularzBan(FormularzBan? daneBan)
		{

			var uzytkownikId = HttpContext.Session.GetString("IdUzytkownika");
			var uzytkownik = _context.KontoUzytkownika.FirstOrDefault(u => u.Id == uzytkownikId);

			
			if (uzytkownik != null)
			{
				if (uzytkownik.LockoutEnabled)
				{
                    
                    if (ModelState.IsValid)
					{
                        Ban ban = new Ban()
                        {
                            Data = daneBan.Data,
                            Dlugosc = daneBan.Dlugosc,
                            Przyczyna = daneBan.Przyczyna,
                            KontoUzytkownikaId = uzytkownik.Id
                        };
                        _context.Ban.Add(ban);
						uzytkownik.LockoutEnabled = false;
						uzytkownik.LockoutEnd = ban.Data.AddDays(ban.Dlugosc);

						_context.Update(uzytkownik);
						_context.SaveChanges();
						return RedirectToAction("Uzytkownik");
					}
					return View(daneBan);
				}
				else
				{
					uzytkownik.LockoutEnabled = true;
					uzytkownik.LockoutEnd = null;
					_context.Update(uzytkownik);
					_context.SaveChanges();
				}
			}


			return RedirectToAction("Uzytkownik");
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Ban(string? opcjaSort, string? searchText)
		{
			string[] opcjeSortowaniaBanow = new string[]
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
			ViewBag.opcjeSortowaniaBanow = opcjeSortowaniaBanow;
			ViewBag.SelectedOptionBan = opcjaSort;

			IQueryable<UzytkownikBan> banyQuery = _context.KontoUzytkownika
				.Join(_context.Ban,
				u => u.Id,
				b => b.KontoUzytkownikaId,
				(u, b) => new
				{
					u.UserName,
					u.Imie,
					u.Nazwisko,
					b.Data,
					b.Dlugosc,
					b.Przyczyna
				}).Select(ub => new UzytkownikBan
				{
					NazwaUzytkownika = ub.UserName,
					Imie = ub.Imie,
					Nazwisko = ub.Nazwisko,
					Data = ub.Data,
					Dlugosc = ub.Dlugosc,
					Przyczyna = ub.Przyczyna
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				banyQuery = banyQuery.Where(ub => ub.NazwaUzytkownika.Contains(searchText) || ub.Imie.Contains(searchText) || ub.Nazwisko.Contains(searchText) || ub.Data.ToString().Contains(searchText) || ub.Dlugosc.ToString().Contains(searchText) || ub.Przyczyna.Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa użytkownika (A-Z)":
					banyQuery = banyQuery.OrderBy(ub => ub.NazwaUzytkownika);
					break;
				case "Nazwa użytkownika (Z-A)":
					banyQuery = banyQuery.OrderByDescending(ub => ub.NazwaUzytkownika);
					break;
				case "Imie (A-Z)":
					banyQuery = banyQuery.OrderBy(ub => ub.Imie);
					break;
				case "Imie (Z-A)":
					banyQuery = banyQuery.OrderByDescending(ub => ub.Imie);
					break;
				case "Nazwisko (A-Z)":
					banyQuery = banyQuery.OrderBy(ub => ub.Nazwisko);
					break;
				case "Nazwisko (Z-A)":
					banyQuery = banyQuery.OrderByDescending(ub => ub.Nazwisko);
					break;
				case "Data rosnąco":
					banyQuery = banyQuery.OrderBy(ub => ub.Data);
					break;
				case "Data malejąco":
					banyQuery = banyQuery.OrderByDescending(ub => ub.Data);
					break;
				case "Liczba dni rosnąco":
					banyQuery = banyQuery.OrderBy(ub => ub.Dlugosc);
					break;
				case "Liczba dni malejąco":
					banyQuery = banyQuery.OrderByDescending(ub => ub.Dlugosc);
					break;
				case "Przyczyna (A-Z)":
					banyQuery = banyQuery.OrderBy(ub => ub.Przyczyna);
					break;
				case "Przyczyna (Z-A)":
					banyQuery = banyQuery.OrderByDescending(ub => ub.Przyczyna);
					break;
				default:
					break;
			}

			List<UzytkownikBan> dane = banyQuery.ToList();

			return View(dane);
		}



       
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> LiveChatAdmin()
        {
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult Limity(string? opcjaSort, string? searchText) 
		{
			string[] opcjeSortowaniaLimitow = new string[]
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
			ViewBag.opcjeSortowaniaLimitow = opcjeSortowaniaLimitow;
			ViewBag.SelectedOptionLimit = opcjaSort;

			IQueryable<UzytkownikLimity> limityQuery = _context.KontoUzytkownika
				.Join(_context.Limit,
				u => u.Id,
				l => l.KontoUzytkownikaId,
				(u, l) => new
				{
					u.UserName,
					u.Imie,
					u.Nazwisko,
					l.Data,
					l.Kwota
				}).Select(ul => new UzytkownikLimity
				{
					NazwaUzytkownika = ul.UserName,
					Nazwisko = ul.Nazwisko,
					Imie = ul.Imie,
					Data = ul.Data,
					Kwota = (float)ul.Kwota
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				limityQuery = limityQuery.Where(ul => ul.NazwaUzytkownika.Contains(searchText) || ul.Imie.Contains(searchText) || ul.Nazwisko.Contains(searchText) || ul.Data.ToString().Contains(searchText) || ul.Kwota.ToString().Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa użytkownika (A-Z)":
					limityQuery = limityQuery.OrderBy(ul => ul.NazwaUzytkownika);
					break;
				case "Nazwa użytkownika (Z-A)":
					limityQuery = limityQuery.OrderByDescending(ul => ul.NazwaUzytkownika);
					break;
				case "Imie (A-Z)":
					limityQuery = limityQuery.OrderBy(ul => ul.Imie);
					break;
				case "Imie (Z-A)":
					limityQuery = limityQuery.OrderByDescending(ul => ul.Imie);
					break;
				case "Nazwisko (A-Z)":
					limityQuery = limityQuery.OrderBy(ul => ul.Nazwisko);
					break;
				case "Nazwisko (Z-A)":
					limityQuery = limityQuery.OrderByDescending(ul => ul.Nazwisko);
					break;
				case "Data rosnąco":
					limityQuery = limityQuery.OrderBy(ul => ul.Data);
					break;
				case "Data malejąco":
					limityQuery = limityQuery.OrderByDescending(ul => ul.Data);
					break;
				case "Kwota rosnąco":
					limityQuery = limityQuery.OrderBy(ul => ul.Kwota);
					break;
				case "Kwota malejąco":
					limityQuery = limityQuery.OrderByDescending(ul => ul.Kwota);
					break;
				default:
					break;
			}

			List<UzytkownikLimity> dane = limityQuery.ToList();

			return View(dane); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Gry(string? opcjaSort, string? searchText)
		{
			string[] opcjeSortowaniaGra = new string[]
			{
				"Domyślnie",
				"Nazwa gry (A-Z)",
				"Nazwa gry (Z-A)",
				"Minimalna stawka rosnąco",
				"Minimalna stawka malejąco"
			};
			ViewBag.opcjeSortowaniaGra = opcjeSortowaniaGra;
			ViewBag.SelectedOptionGra = opcjaSort;
			IQueryable<Gra> gryQuery = _context.Gra;

			if (!string.IsNullOrEmpty(searchText))
			{
				gryQuery = gryQuery.Where(g => g.Nazwa.Contains(searchText) || g.MinStawka.ToString().Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa gry (A-Z)":
					gryQuery = gryQuery.OrderBy(g => g.Nazwa);
					break;
				case "Nazwa gry (Z-A)":
					gryQuery = gryQuery.OrderByDescending(g => g.Nazwa);
					break;
				case "Minimalna stawka rosnąco":
					gryQuery = gryQuery.OrderBy(g => g.MinStawka);
					break;
				case "Minimalna stawka malejąco":
					gryQuery = gryQuery.OrderByDescending(g => g.MinStawka);
					break;
				default:
					break;
			}

			List<Gra> dane = gryQuery.ToList();

			return View(dane); 
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult AktualizacjaStawki(int idGry, int nowaStawka)
		{
			var gra = _context.Gra.Find(idGry);
			if (gra != null)
			{
				gra.MinStawka = nowaStawka;
				_context.SaveChanges();
			}
			return RedirectToAction("Gry");
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Transakcje(string? opcjaSort, string? searchText)
		{
			string[] opcjeSortowaniaTransakcja = new string[]
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
			ViewBag.opcjeSortowaniaTransakcji = opcjeSortowaniaTransakcja;
			ViewBag.SelectedOptionTransakcja = opcjaSort;

			IQueryable<UzytkownikTransakcja> transakcjaQuery = _context.KontoUzytkownika
				.Join(_context.Transakcja,
				u => u.Id,
				t => t.KontoUzytkownikaId,
				(u, t) => new
				{
					u.UserName,
					u.Nazwisko,
					u.Imie,
					t.Data,
					t.Kwota,
					t.Typ
				}).Select(ut => new UzytkownikTransakcja
				{
					NazwaUzytkownika = ut.UserName,
					Nazwisko = ut.Nazwisko,
					Imie = ut.Imie,
					Data = ut.Data,
					Kwota = (float)ut.Kwota,
					Typ = ut.Typ
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				transakcjaQuery = transakcjaQuery.Where(ut => ut.NazwaUzytkownika.Contains(searchText) || ut.Imie.Contains(searchText) || ut.Nazwisko.Contains(searchText) || ut.Data.ToString().Contains(searchText) || ut.Kwota.ToString().Contains(searchText) || ut.Typ.Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa użytkownika (A-Z)":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.NazwaUzytkownika);
					break;
				case "Nazwa użytkownika (Z-A)":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.NazwaUzytkownika);
					break;
				case "Imie (A-Z)":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.Imie);
					break;
				case "Imie (Z-A)":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.Imie);
					break;
				case "Nazwisko (A-Z)":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.Nazwisko);
					break;
				case "Nazwisko (Z-A)":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.Nazwisko);
					break;
				case "Data rosnąco":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.Data);
					break;
				case "Data malejąco":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.Data);
					break;
				case "Kwota rosnąco":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.Kwota);
					break;
				case "Kwota malejąco":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.Kwota);
					break;
				case "Typ (A-Z)":
					transakcjaQuery = transakcjaQuery.OrderBy(ut => ut.Typ);
					break;
				case "Typ (Z-A)":
					transakcjaQuery = transakcjaQuery.OrderByDescending(ut => ut.Typ);
					break;
				default:
					break;
			}

			List<UzytkownikTransakcja> dane = transakcjaQuery.ToList();

			return View(dane);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult GraKonto(string? opcjaSort, string? searchText)
		{
			string[] opcjeSortowaniaGraKonto = new string[]
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
			ViewBag.opcjeSortowaniaGraKonto = opcjeSortowaniaGraKonto;
			ViewBag.SelectedOptionGraKonto = opcjaSort;

			IQueryable<UzytkownikGraKonta> graKontoQuery = _context.KontoUzytkownika
				.Join(_context.GraKonto,
				u => u.Id,
				gk => gk.KontoUzytkownikaId,
				(u, gk) => new
				{
					u.UserName,
					u.Imie,
					u.Nazwisko,
					gk.IleWygrano,
					gk.IlePostawiono,
					gk.Czas,
					gk.IdGry
				}).Join(_context.Gra,
				ugk => ugk.IdGry,
				g => g.IdGry,
				(ugk, g) => new
				{
					ugk.UserName,
					ugk.Imie,
					ugk.Nazwisko,
					ugk.IleWygrano,
					ugk.IlePostawiono,
					ugk.Czas,
					g.Nazwa
				}).Select(x => new UzytkownikGraKonta
				{
					NazwaUzytkownika = x.UserName,
					Nazwisko = x.Nazwisko,
					Imie = x.Imie,
					IleWygrano = (float)x.IleWygrano,
					IlePostawiono = (float)x.IlePostawiono,
					Czas = (DateTime)x.Czas,
					NazwaGry = x.Nazwa
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				graKontoQuery = graKontoQuery.Where(gk => gk.NazwaUzytkownika.Contains(searchText) || gk.Imie.Contains(searchText) || gk.Nazwisko.Contains(searchText) || gk.IleWygrano.ToString().Contains(searchText) || gk.IlePostawiono.ToString().Contains(searchText) || gk.Czas.ToString().Contains(searchText) || gk.NazwaGry.Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa użytkownika (A-Z)":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.NazwaUzytkownika);
					break;
				case "Nazwa użytkownika (Z-A)":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.NazwaUzytkownika);
					break;
				case "Imie (A-Z)":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.Imie);
					break;
				case "Imie (Z-A)":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.Imie);
					break;
				case "Nazwisko (A-Z)":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.Nazwisko);
					break;
				case "Nazwisko (Z-A)":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.Nazwisko);
					break;
				case "Wygrana rosnąco":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.IleWygrano);
					break;
				case "Wygrana malejąco":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.IleWygrano);
					break;
				case "Postawienie rosnąco":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.IlePostawiono);
					break;
				case "Postawienie malejąco":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.IlePostawiono);
					break;
				case "Czas rosnąco":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.Czas);
					break;
				case "Czas malejąco":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.Czas);
					break;
				case "Nazwa gry (A-Z)":
					graKontoQuery = graKontoQuery.OrderBy(gk => gk.NazwaGry);
					break;
				case "Nazwa gry (Z-A)":
					graKontoQuery = graKontoQuery.OrderByDescending(gk => gk.NazwaGry);
					break;
				default:
					break;
			}

			List<UzytkownikGraKonta> dane = graKontoQuery.ToList();
			return View(dane);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult Zgloszenie(string? opcjaSort, string? searchText)
		{
			string[] opcjeSortowaniaZgloszenie = new string[]
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
			ViewBag.opcjeSortowaniaZgloszenie = opcjeSortowaniaZgloszenie;
			ViewBag.SelectedOptionZgloszenie = opcjaSort;

			IQueryable<UzytkownikZgloszenie> zgloszenieQuery = _context.KontoUzytkownika
				.Join(_context.Zgloszenie,
				u => u.Id,
				z => z.KontoUzytkownikaId,
				(u, z) => new
				{
					u.UserName,
					u.Nazwisko,
					u.Imie,
					z.Tytul,
					z.Status,
					z.Data,
					z.IdZgloszenia
				}).Select(uz => new UzytkownikZgloszenie
				{
					IdZgloszenia = uz.IdZgloszenia,
					NazwaUzytkownika = uz.UserName,
					Nazwisko = uz.Nazwisko,
					Imie = uz.Imie,
					Status = uz.Status,
					Data = uz.Data,
					Tytul = uz.Tytul
				});

			if (!string.IsNullOrEmpty(searchText))
			{
				zgloszenieQuery = zgloszenieQuery.Where(uz => uz.NazwaUzytkownika.Contains(searchText) || uz.Imie.Contains(searchText) || uz.Nazwisko.Contains(searchText) || uz.Status.Contains(searchText) || uz.Tytul.Contains(searchText) || uz.Data.ToString().Contains(searchText));
			}

			switch (opcjaSort)
			{
				case "Nazwa użytkownika (A-Z)":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.NazwaUzytkownika);
					break;
				case "Nazwa użytkownika (Z-A)":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.NazwaUzytkownika);
					break;
				case "Imie (A-Z)":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.Imie);
					break;
				case "Imie (Z-A)":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.Imie);
					break;
				case "Nazwisko (A-Z)":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.Nazwisko);
					break;
				case "Nazwisko (Z-A)":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.Nazwisko);
					break;
				case "Data rosnąco":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.Data);
					break;
				case "Data malejąco":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.Data);
					break;
				case "Status (A-Z)":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.Status);
					break;
				case "Status (Z-A)":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.Status);
					break;
				case "Tytuł (A-Z)":
					zgloszenieQuery = zgloszenieQuery.OrderBy(uz => uz.Tytul);
					break;
				case "Tytuł (Z-A)":
					zgloszenieQuery = zgloszenieQuery.OrderByDescending(uz => uz.Tytul);
					break;
				default:
					break;
			}

			List<UzytkownikZgloszenie> dane = zgloszenieQuery.ToList();

			return View(dane);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult SzczegolyZgloszenia(int zgloszenieId)
		{
			var zgloszenie = _context.Zgloszenie.Find(zgloszenieId);
			if (zgloszenie == null)
			{
				return NotFound();
			}
			return View(zgloszenie);
		}

        [Authorize(Roles = "Administrator")]
        public IActionResult ZmienStatus(int zgloszenieId, string nowyStatus)
		{
			var zgloszenie = _context.Zgloszenie.Find(zgloszenieId);
            if (zgloszenie == null)
            {
                return NotFound();
            }

			zgloszenie.Status = nowyStatus;
			_context.SaveChanges();
			return RedirectToAction("Zgloszenie");
		}


        [Authorize(Roles = "Administrator")]
        public IActionResult HealthCheck()
        {
          
            return View();
        }

    }
}
