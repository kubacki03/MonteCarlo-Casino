
(function () {
    "use strict";
    let czyWyswietlono = false;
    const items = [
        "7️⃣",
        "❌",
        "🍓",
        "🍋",
        "🍉",
        "🍒",
        "💵",
        "🍊",
        "🍎"
    ];

    const kolumny = document.querySelectorAll(".kolumna");
    const wynik = document.querySelector(".wynik");
    document.querySelector("#losuj").addEventListener("click", function (event) {
        const stawka = parseFloat(document.querySelector("#stawka").value);

        if (isNaN(stawka) || stawka <= 0 || !Number.isInteger(Number(stawka))) {
            alert("Proszę wprowadzić liczbę naturalną większą niż 0!");
            event.preventDefault();
            return;
        }
        init();
        Losowanie();
        WyczyscWynik();
        
    });

    async function Losowanie() {
        init(false, 1, 2);

        for (const kolumna of kolumny) {
            const elementy = kolumna.querySelector(".elementy");
            const czasAnimacji = parseInt(elementy.style.transitionDuration);
            elementy.style.transform = "translateY(0)";
            await new Promise((resolve) => setTimeout(resolve, czasAnimacji * 100));
        }
        ResetCzyWyswietlono();
        CzyWygrano();
        setTimeout(() => {
            console.log("Przekierowanie po 2 sekundach...");
            window.location.href = "/Home/Slotsy";
        }, 2000);
    }

    async function CzyWygrano() {
        if (czyWyswietlono) return;

        const arrWyniki = Array.from(kolumny).map(kolumna => {
            const glownyElement = kolumna.querySelector(".element");
            return glownyElement.textContent;
        });

        await new Promise(resolve => setTimeout(resolve, 1900));

        if (czyWyswietlono) return;

        czyWyswietlono = true;

        const wygrana = arrWyniki.every(symbol => symbol === arrWyniki[0]);

        if (wygrana) {
            wynik.textContent = "Gratulacje! Wygrałeś!";
            wynik.style.color = "green";
        } else {
            wynik.textContent = "Spróbuj ponownie!";
            wynik.style.color = "red";
        }

        const stawka = parseFloat(document.querySelector("#stawka").value) || 0;

        fetch('/Home/ZwrocWynik', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ wygrana, stawka })
        })
            .then(response => response.json())
            .then(data => console.log(data))
            .catch(error => console.error('Błąd:', error));
    }




    function ResetCzyWyswietlono() {
        czyWyswietlono = false;
        wynik.textContent = "";
    }

    function WyczyscWynik() {
        wynik.textContent = "";
    }

    function init(pierwszaInicjalizacja = true, grupy = 1, czasAnimacji = 1) {
        for (const kolumna of kolumny) {
            if (pierwszaInicjalizacja) {
                kolumna.dataset.spinned = "0";
            } else if (kolumna.dataset.spinned === "1") {
                return;
            }

            const elementy = kolumna.querySelector(".elementy");
            const klonowanieElementow = elementy.cloneNode(false);

            const pula = ["❓"];
            if (!pierwszaInicjalizacja) {
                const arr = [];
                for (let n = 0; n < (grupy > 0 ? grupy : 1); n++) {
                    arr.push(...items);
                }
                pula.push(...Tasowanie(arr));

                klonowanieElementow.addEventListener(
                    "transitionstart",
                    function () {
                        kolumna.dataset.spinned = "1";
                        this.querySelectorAll(".element").forEach((element) => {
                            element.style.filter = "blur(1px)";
                        });
                    },
                    { once: true }
                );

                klonowanieElementow.addEventListener(
                    "transitionend",
                    function () {
                        this.querySelectorAll(".element").forEach((element, index) => {
                            element.style.filter = "blur(0)";
                            if (index > 0) this.removeChild(element);
                        });

                        if (Array.from(kolumny).every(kolumna => kolumna.dataset.spinned === "1")) {
                            CzyWygrano();
                        }
                    },
                    { once: true }
                );
            }

            for (let i = pula.length - 1; i >= 0; i--) {
                const element = document.createElement("div");
                element.classList.add("element");
                element.style.width = kolumna.clientWidth + "px";
                element.style.height = kolumna.clientHeight + "px";
                element.textContent = pula[i];
                klonowanieElementow.appendChild(element);
            }
            klonowanieElementow.style.transitionDuration = `${czasAnimacji > 0 ? czasAnimacji : 1}s`;
            klonowanieElementow.style.transform = `translateY(-${kolumna.clientHeight * (pula.length - 1)
                }px)`;
            kolumna.replaceChild(klonowanieElementow, elementy);
        }
    }

    function Tasowanie([...arr]) {
        let m = arr.length;
        while (m) {
            const i = Math.floor(Math.random() * m--);
            [arr[m], arr[i]] = [arr[i], arr[m]];
        }
        return arr;
    }

    init();
})();