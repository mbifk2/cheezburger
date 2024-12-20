# Cheezburger
## Svarbi informacija
- Šis darbas atliktas Kauno Technologijos Universiteto informatikos fakulteto moduliui `T120B165 Saityno taikomųjų programų projektavimas`.
- Darbą atliko IFK-2 grupės studentas Matas Bingelis.
- Šioje saugykloje laikomas tik API projektas. UI projektas yra šioje saugykloje: https://github.com/matbin532/cheez-frontend
## Aprašymas
- Cheezburger yra paprastas internetinis forumas su temomis(subforumais), gijomis bei komentarais.
- Forumo API yra sukurtas pasiremiant REST principais.
- API specifikacija patalpina faile `openapi.yml`.
- Projekto realizacijos nuoroda: https://zealous-plant-03aaed103.4.azurestaticapps.net/

## Realizacijos priemonės
- API: C#
- Duomenų bazė: Microsoft SQL Server (Azure SQL)
- UI: Vue.js (Vue3)
- Talpinimas saityne: Microsoft Azure

## Sistemos paskirtis
- Bendrauti ir diskutuoti su kitais, pasidalinant savo mintimis ir nuomonėmis.

## Funkciniai reikalavimai
- Vartotojų autentifikacija/autorizacija(Naudojama JWT)
- Neprisijungę vartotojai gali tik matyti forumo objektus, t.y. siųsti tik GET užklausas.
- Prisijungę vartotojai gali kurti temas, gijas bei komentarus.
- Prisijungę vartotojai gali redaguoti/ištrinti savo sukurtus objektus bei savo asmeninę informaciją.
- Administratoriai gali laisvai redaguoti/ištrinti bet kokį sistemos objektą.

## Sistemos architektūra
![image](https://github.com/user-attachments/assets/b400e5ad-1f04-474d-a2a2-8fd3d945c618)


## Vartotojo sąsajos projektas 
### Navigacijos meniu
- Eskizas:
![image](https://github.com/user-attachments/assets/1568c675-c69d-4e22-bdc1-94c85a2e5a0a)
- Realizacija:
![image](https://github.com/user-attachments/assets/bf678d7f-d510-4759-816e-4c4d48c1f038)
### Temų sąrašas
- Eskizas:
![image](https://github.com/user-attachments/assets/67a6debc-e5d5-40f4-8410-9e04242999c9)
- Realizacija:
![image](https://github.com/user-attachments/assets/c0c1025c-10e4-4e78-867b-7d05c21ab1fe)
### Forumo gija
- Eskizas:
![image](https://github.com/user-attachments/assets/2c9d5f63-4bee-4e70-9dcd-071ef901d646)
- Realizacija:
![image](https://github.com/user-attachments/assets/69ad974a-afbe-45e9-8016-bd0c059f371e)

## Projekto išvados
- Projektą pavyko beveik pilnai realizuoti, tik pritrūko laiko kai kurioms nedidelėms UI detalėms kaip ikonoms vietoj teksto ant mygtukų.
- Projektas buvo labai įdomus, tikrai pamokino apie saityno programas bei interneto svetainių publikavimą.
- Mėgstamiausias viso semestro darbas. Net gaila, kad kiti moduliai atėmė laiką iš šio projekto.
- Išmokau apie REST API, Azure bei JavaScript.

