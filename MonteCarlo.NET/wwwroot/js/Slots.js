(function () {
    "use strict";

    
    const items = ["7️⃣", "❌", "🍓", "🍋", "🍉", "🍒", "💵", "🍊", "🍎"];

    const kolumny = document.querySelectorAll(".kolumna");
    const wynik = document.querySelector(".wynik");
    const przycisk = document.querySelector("#losuj");
    let trwaLosowanie = false;

    przycisk.addEventListener("click", async function () {
        if (trwaLosowanie) return;

        const stawka = Number(document.querySelector("#stawka").value);
        if (!Number.isInteger(stawka) || stawka <= 0) {
            alert("Proszę wprowadzić liczbę naturalną większą niż 0!");
            return;
        }

        trwaLosowanie = true;
        przycisk.disabled = true;
        wynik.textContent = "";

        try {
            const spin = await requestSpin(stawka);
            if (!spin.succeeded) {
                pokazWynik(spin.error, "red");
                trwaLosowanie = false;
                przycisk.disabled = false;
                return;
            }

            await animuj(spin.symbols);
            if (spin.won) {
                pokazWynik(`Gratulacje! Wygrałeś ${spin.prize}!`, "green");
            } else {
                pokazWynik("Spróbuj ponownie!", "red");
            }
            // reload so the balance in the layout is refreshed
            setTimeout(() => { window.location.href = "/Home/Slots"; }, 2000);
        } catch (error) {
            console.error(error);
            pokazWynik("Wystąpił błąd, spróbuj ponownie.", "red");
            trwaLosowanie = false;
            przycisk.disabled = false;
        }
    });

    async function requestSpin(stake) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const response = await fetch("/Home/SpinSlots", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify({ stake })
        });
        if (!response.ok) {
            throw new Error(`Spin request failed: ${response.status}`);
        }
        return response.json();
    }

    function pokazWynik(tekst, kolor) {
        wynik.textContent = tekst;
        wynik.style.color = kolor;
    }

    async function animuj(symbole) {
        init(false, symbole);

        for (const kolumna of kolumny) {
            const elementy = kolumna.querySelector(".elementy");
            const czasAnimacji = parseInt(elementy.style.transitionDuration);
            elementy.style.transform = "translateY(0)";
            await new Promise((resolve) => setTimeout(resolve, czasAnimacji * 100));
        }
        await new Promise((resolve) => setTimeout(resolve, 1900));
    }

    function init(pierwszaInicjalizacja = true, symbole = []) {
        kolumny.forEach((kolumna, indeks) => {
            const elementy = kolumna.querySelector(".elementy");
            const klonowanieElementow = elementy.cloneNode(false);

            const pula = ["❓"];
            if (!pierwszaInicjalizacja) {
                const wymieszane = Tasowanie(items);
                // the last element ends up visible once the reel stops
                wymieszane[wymieszane.length - 1] = symbole[indeks];
                pula.push(...wymieszane);

                klonowanieElementow.addEventListener(
                    "transitionstart",
                    function () {
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
            klonowanieElementow.style.transitionDuration = "2s";
            klonowanieElementow.style.transform = `translateY(-${kolumna.clientHeight * (pula.length - 1)}px)`;
            kolumna.replaceChild(klonowanieElementow, elementy);
        });
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
