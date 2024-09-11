# **CINEMASTER**
## _Sistemos paskirtis_
Pagridinė paskirtis yra suteikti vartotojams patogią platformą laisvai surasti visą reikiamą informaciją apie filmus,
galimybė peržiūrėti kiek ir kokių filmų žiūrėjo ir kiti sistemos vartotojai bei kaip juos vertina, atsiliepimus. Pagal tai kokius dažniausiai filmus yra žiūrima
svetainė pasiūlys ką žiūrėti toliau. Sistema leis išsifiltruoti pagal norimus žanrus, įvertinimą, šalį, populiarumą, išleidimo datą ir kt.
## _Funkciniai reikalavimai_
### **Svečias**

    Matyti filmų aprašymus, apžvalgas, įvertinimus, aktorių ir režisierių informaciją.
    Galimybė filtruoti filmus pagal žanrą, įvertinimą, populiarumą, datą ir pan.
    Galimybė matyti vartotojų filmų sąrašus ir jų informaciją.
    Galimybė vykdyti paieškos užklausą.
    Galimybė kurti paskyrą.

### **Vartotojas**

    Galimybė prisijungti ir atsijungti.
    Filmų pridėjimas į norimų žiūrėti sąrašą (watchlist).
    Galimybė pridėti filmus į peržiūrėtų filmų sąrašą.
    Filmų įvertinimas
    Statistika apie peržiūrėtus filmus (pvz., kiek filmų peržiūrėjo per metus, mėnesį, bendrą peržiūrėtų filmų laiką).
    Galimybė sekti kitus vartotojus ir matyti jų veiklą (ką žiūri, ką vertina, ką rekomenduoja).
    Kurti filmų sąrašus (pvz., „Klasikiniai siaubo filmai“).
    Matyti personalizuotas rekomendacijas, remiantis peržiūrėtais ir įvertintais filmais.

### **Administratorius**
    
    Filmų duomenų bazės valdymas: filmų pridėjimas, redagavimas, ištrynimas.
    Vartotojų veiklos stebėjimas ir moderavimas (pvz., netinkamų komentarų ar apžvalgų šalinimas).
    Vartotojų paskyrų valdymas
    Statistinė analizė: galimybė matyti vartotojų aktyvumą, populiariausius filmus, dažniausiai žiūrimus žanrus ir t.t.
    Sisteminių pranešimų siuntimas visiems vartotojams (pvz., naujienos apie atnaujinimus, taisyklių pasikeitimus ir pan.).
### Technologijos
- Angular framework (Front-end)
  - Naudoja Typescript programavimo kalbą
  - Sukurtas Google kompanijos
  - Tinka single-page puslapiams kurti
  - UI elementai susideda iš komponentų, kuriuos galima daugybę kartų panaudoti
  - Two-way data bidning, automatiškai atnaujina VIEW ir MODEL informaciją.
- ASP .NET Core (Back-end)
  - Cross-platform
  - Palaiko MVC modelį
  - Palaiko NuGet paketus
- PostgreSQL (DBVS)
  - Open source reliacinė duomeneų bazių valdymo sistema
  - Palaiko sudėtingas SQL užklausas, turi daug papildomų funkcijų
