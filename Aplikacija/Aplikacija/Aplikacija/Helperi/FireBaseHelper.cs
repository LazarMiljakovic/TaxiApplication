using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;
using Firebase.Database.Query;
using Aplikacija.Modeli;
using System.Threading.Tasks;
using System.Linq;

namespace Aplikacija.Helperi
{
    public class FireBaseHelper
    {

        public static string FB = "1fKOogoINaf8GtfcitE87Tw5hGX7zGGMekTqslN0";
        FirebaseClient firebase = new FirebaseClient("https://taxiapp-6909e-default-rtdb.europe-west1.firebasedatabase.app/",new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(FB)});
        

        public async Task PozoviVoznju(Voznja voznja)
        {
            await firebase
              .Child("Voznja")
              .PostAsync(new Voznja() { Pocetak = voznja.Pocetak, Kraj=voznja.Kraj,cena = voznja.cena , kilometraza=voznja.kilometraza, vreme= voznja.vreme, Napomena = voznja.Napomena,ImeiPrezime = voznja.ImeiPrezime,BrojTelefonaVozaca = voznja.BrojTelefonaVozaca});
        }
        public async Task PrekiniPretragu(Voznja voznja)
        {
            var brisiVoznju = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak&& a.Object.Kraj == voznja.Kraj&& a.Object.kilometraza==voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).FirstOrDefault();
            await firebase.Child("Voznja").Child(brisiVoznju.Key).DeleteAsync();

        }
        public async Task RegistrujNalog(User user)
        {
            await firebase
              .Child("User")
              .PostAsync(new User() { BrojTelefona = user.BrojTelefona, Ime = user.Ime,Prezime = user.Prezime,Email = user.Email,Sifra= user.Sifra });

        }
        public async Task<User> VratiUsera(int brTelefona,string sifra)
        {
            var u =  (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(a=> a.Object.BrojTelefona == brTelefona&& a.Object.Sifra == sifra).Select(i => new User()
              { 
                  BrojTelefona = i.Object.BrojTelefona,
                  Ime = i.Object.Ime,
                  Prezime = i.Object.Prezime,
                  Email = i.Object.Email,
                  Sifra = i.Object.Sifra
                  
              });
            User user = new User();
            user = u.FirstOrDefault();
            
            return user;
        }

        public async Task<Vozac> VratiVozaca(int id, string sifra)
        {
            var u = (await firebase
              .Child("Vozac")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == id && a.Object.sifra == sifra).Select(i => new Vozac()
              {
                  id = i.Object.id,
                  Ime = i.Object.Ime,
                  Prezime = i.Object.Prezime,
                  Registracija = i.Object.Registracija,
                  Vozilo = i.Object.Vozilo,
                  Vrsta = i.Object.Vrsta,
                  sifra = i.Object.sifra,


              });
            Vozac vozac = new Vozac();
            vozac = u.FirstOrDefault();

            return vozac;
        }

        public async Task RegistrujNalogVozaca(Vozac vozac)
        {
            await firebase
              .Child("Vozac")
              .PostAsync(new Vozac() {
                  id = vozac.id,
                  Ime = vozac.Ime,
                  Prezime = vozac.Prezime,
                  Registracija = vozac.Registracija,
                  Vozilo = vozac.Vozilo,
                  Vrsta = vozac.Vrsta,
                  sifra = vozac.sifra,
              });

        }

        public async Task<Sef> VratiSefa(int id, string sifra)
        {
            var u = (await firebase
              .Child("Sef")
              .OnceAsync<Sef>()).Where(a => a.Object.id == id && a.Object.Sifra == sifra).Select(i => new Sef()
              {
                  id = i.Object.id,
                  Ime = i.Object.Ime,
                  Sifra = i.Object.Sifra,


              });
            Sef sef = new Sef();
            sef = u.FirstOrDefault();

            return sef;
        }

        public async Task RegistrujNalogSefa(Sef sef)
        {
            await firebase
              .Child("Sef")
              .PostAsync(new Sef() {
                  id = sef.id,
                  Ime = sef.Ime,
                  Sifra = sef.Sifra,
              });

        }
        public async Task<Administrator> VratiAdm(int id, string sifra)
        {
            var u = (await firebase
              .Child("Administrator")
              .OnceAsync<Administrator>()).Where(a => a.Object.id == id && a.Object.Sifra == sifra).Select(i => new Administrator()
              {
                  id = i.Object.id,
                  Ime = i.Object.Ime,
                  Sifra = i.Object.Sifra,


              });
            Administrator administrator = new Administrator();
            administrator = u.FirstOrDefault();

            return administrator;
        }

        public async Task RegistrujNalogAdm(Administrator administrator)
        {
            await firebase
              .Child("Administrator")
              .PostAsync(new Administrator()
              {
                  id = administrator.id,
                  Ime = administrator.Ime,
                  Sifra = administrator.Sifra,
              });

        }
        public async Task spremniVozac(Vozac vozac)
        {
            await firebase
              .Child("SpremniVozaci")
              .PostAsync(new Vozac()
              {
                  id = vozac.id,
                  Ime = vozac.Ime,
                  Prezime = vozac.Prezime,
                  Registracija = vozac.Registracija,
                  Vozilo = vozac.Vozilo,
                  Vrsta = vozac.Vrsta,
                  Spreman = vozac.Spreman,
                  laTrenutna = vozac.laTrenutna,
                  loTrenutna = vozac.loTrenutna

              });

        }
        public async Task OfflineVozac(Vozac vozac)
        {
            var brisiOnline = (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == vozac.id).FirstOrDefault();
            await firebase.Child("SpremniVozaci").Child(brisiOnline.Key).DeleteAsync();

        }
        public async Task<List<Vozac>> VratiSveSpremneVozace()
        {

            return (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.Spreman == "Spreman").Select(vozac => new Vozac
              {
                  id = vozac.Object.id,
                  Ime = vozac.Object.Ime,
                  Prezime = vozac.Object.Prezime,
                  Registracija = vozac.Object.Registracija,
                  Vozilo = vozac.Object.Vozilo,
                  Vrsta = vozac.Object.Vrsta,
                  Spreman = vozac.Object.Spreman,
                  laTrenutna = vozac.Object.laTrenutna,
                  loTrenutna = vozac.Object.loTrenutna
              }).ToList();
        }

        public async Task UpdateVoznju(int vozacId, string voznjaId)
        {
            var saljiReq = (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == vozacId).FirstOrDefault();

            await firebase
              .Child("SpremniVozaci")
              .Child(saljiReq.Key)
              .PutAsync(new Vozac() { Spreman = voznjaId, id = saljiReq.Object.id, Ime = saljiReq.Object.Ime, Prezime = saljiReq.Object.Prezime, Registracija = saljiReq.Object.Registracija, laTrenutna = saljiReq.Object.laTrenutna, loTrenutna = saljiReq.Object.loTrenutna, Vozilo = saljiReq.Object.Vozilo,Vrsta= saljiReq.Object.Vrsta }) ;
        }
        public async Task<string> VratiVoznju(Voznja voznja)
        {
            var u = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme).Select(i => i.Key);

            string idVoznje = u.FirstOrDefault();

            return idVoznje;
        }
        public async Task<string> VratiStatus(int id)
        {
            var u = (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == id).Select(i => i.Object.Spreman);
           
            string spr = u.FirstOrDefault();

            return spr;
        }
        public async Task<Voznja> VratiVoznjuVozacu(string voznja)
        {
            var u = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Key == voznja).Select(i => new Voznja()
              {
                  Pocetak = i.Object.Pocetak,
                  Kraj = i.Object.Kraj,
                  cena = i.Object.cena,
                  Napomena = i.Object.Napomena,
                  kilometraza = i.Object.kilometraza,
                  BrojTelefonaVozaca = i.Object.BrojTelefonaVozaca,
                  ImeiPrezime = i.Object.ImeiPrezime,
                  vreme = i.Object.vreme,
              });

            Voznja v = u.FirstOrDefault();

            return v;
        }
        public async Task UpdateStatus(int vozacId,string status)
        {
            var saljiReq = (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == vozacId).FirstOrDefault();

            if (saljiReq != null)
            {
                await firebase
              .Child("SpremniVozaci")
              .Child(saljiReq.Key)
              .PutAsync(new Vozac() { Spreman = status, id = saljiReq.Object.id, Ime = saljiReq.Object.Ime, Prezime = saljiReq.Object.Prezime, Registracija = saljiReq.Object.Registracija, laTrenutna = saljiReq.Object.laTrenutna, loTrenutna = saljiReq.Object.loTrenutna, Vozilo = saljiReq.Object.Vozilo, Vrsta = saljiReq.Object.Vrsta });
            }
            
        }

        public async Task PrihvatiVoznju(Voznja voznja, string status)
        {
            var saljiReq = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).FirstOrDefault();

            await firebase
              .Child("Voznja")
              .Child(saljiReq.Key)
              .PutAsync(new Voznja() { Pocetak = voznja.Pocetak,idVozaca = voznja.idVozaca, Kraj = voznja.Kraj, cena = voznja.cena, kilometraza = voznja.kilometraza, vreme = voznja.vreme, Napomena = voznja.Napomena, ImeiPrezime = voznja.ImeiPrezime, BrojTelefonaVozaca = voznja.BrojTelefonaVozaca, Prahivacena = status });
        }
        public async Task DodajVozacaVoznji(Voznja voznja, int id)
        {
            var saljiReq = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).FirstOrDefault();

            await firebase
              .Child("Voznja")
              .Child(saljiReq.Key)
              .PutAsync(new Voznja() { Pocetak = voznja.Pocetak, Kraj = voznja.Kraj, cena = voznja.cena, kilometraza = voznja.kilometraza, vreme = voznja.vreme, Napomena = voznja.Napomena, ImeiPrezime = voznja.ImeiPrezime, BrojTelefonaVozaca = voznja.BrojTelefonaVozaca, idVozaca = id });
        }
        public async Task<string> StatusPrihvacenje(Voznja voznja)
        {
            var u = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).Select(i => i.Object.Prahivacena);
            string s = u.FirstOrDefault();

            return s;

        }


        public async Task Updatelokaciju(int vozacId, double la,double lo)
        {
            var saljiReq = (await firebase
              .Child("SpremniVozaci")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == vozacId).FirstOrDefault();

            await firebase
              .Child("SpremniVozaci")
              .Child(saljiReq.Key)
              .PutAsync(new Vozac() { laTrenutna = la,loTrenutna =lo });
        }
        public async Task<Voznja> VratiVoznjuKlijentu(Voznja voznja)
        {
            var u = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).Select(i => new Voznja()
              {
                  Pocetak = i.Object.Pocetak,
                  Kraj = i.Object.Kraj,
                  cena = i.Object.cena,
                  Napomena = i.Object.Napomena,
                  kilometraza = i.Object.kilometraza,
                  BrojTelefonaVozaca = i.Object.BrojTelefonaVozaca,
                  ImeiPrezime = i.Object.ImeiPrezime,
                  vreme = i.Object.vreme,
                  idVozaca = i.Object.idVozaca,
                  
              });

            Voznja v = u.FirstOrDefault();

            return v;
        }
        public async Task Oceni(int v,int z,string nap)
        {
            await firebase.Child("Ocene").PostAsync(new RecenzijeMod() {vozac = v,zvezde = z,napomena=nap });
               
        }

        public async Task<List<Voznja>> VratiSveVoznjeVozaca(Vozac v)
        {

            return (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.idVozaca == v.id && a.Object.Prahivacena =="Zavrsena").Select(i => new Voznja
              {
                  Pocetak = i.Object.Pocetak,
                  Kraj = i.Object.Kraj,
                  cena = i.Object.cena,
                  Napomena = i.Object.Napomena,
                  kilometraza = i.Object.kilometraza,
                  vreme = i.Object.vreme,
                  idVozaca = i.Object.idVozaca,

              }).ToList();
        }
        public async Task UbaciVoznjuFin(Voznja voznja)
        {
            var saljiReq = (await firebase
              .Child("Voznja")
              .OnceAsync<Voznja>()).Where(a => a.Object.Pocetak == voznja.Pocetak && a.Object.Kraj == voznja.Kraj && a.Object.kilometraza == voznja.kilometraza && a.Object.vreme == voznja.vreme && a.Object.ImeiPrezime == voznja.ImeiPrezime && a.Object.BrojTelefonaVozaca == voznja.BrojTelefonaVozaca).FirstOrDefault();

            await firebase
              .Child("Voznja")
              .Child(saljiReq.Key)
              .PutAsync(new Voznja() { Pocetak = voznja.Pocetak, Kraj = voznja.Kraj, cena = voznja.cena, kilometraza = voznja.kilometraza, vreme = voznja.vreme, Napomena = voznja.Napomena, ImeiPrezime = voznja.ImeiPrezime, BrojTelefonaVozaca = voznja.BrojTelefonaVozaca, Prahivacena = voznja.Prahivacena, idVozaca = voznja.idVozaca });
        }

        public async Task<List<Administrator>> VratiAdmList()
        {
            return (await firebase
              .Child("Administrator")
              .OnceAsync<Administrator>()).Select(i => new Administrator()
              {
                  id = i.Object.id,
                  Ime = i.Object.Ime,
                  Sifra = i.Object.Sifra,


              }).ToList();
            
        }
        public async Task<List<Sef>> VratiSefList()
        {
            return (await firebase
              .Child("Sef")
              .OnceAsync<Sef>()).Select(i => new Sef()
              {
                  id = i.Object.id,
                  Ime = i.Object.Ime,
                  Sifra = i.Object.Sifra,


              }).ToList();

        }
        public async Task<List<Vozac>> VratiVozList()
        {
            return (await firebase
              .Child("Vozac")
              .OnceAsync<Vozac>()).Select(vozac => new Vozac()
              {
                  id = vozac.Object.id,
                  Ime = vozac.Object.Ime,
                  Prezime = vozac.Object.Prezime,
                  Registracija = vozac.Object.Registracija,
                  Vozilo = vozac.Object.Vozilo,
                  Vrsta = vozac.Object.Vrsta,
                  
                  

              }).ToList();

        }

        public async Task ObrisiAdmina(Administrator adm)
        {
            var brisi = (await firebase
              .Child("Administrator")
              .OnceAsync<Administrator>()).Where(a => a.Object.id == adm.id).FirstOrDefault();
            await firebase.Child("Administrator").Child(brisi.Key).DeleteAsync();

        }
        public async Task ObrisiSefa(Sef sef)
        {
            var brisi = (await firebase
              .Child("Sef")
              .OnceAsync<Sef>()).Where(a => a.Object.id == sef.id).FirstOrDefault();
            await firebase.Child("Sef").Child(brisi.Key).DeleteAsync();

        }
        public async Task ObrisiVozaca(Vozac vozac)
        {
            var brisi = (await firebase
              .Child("Vozac")
              .OnceAsync<Vozac>()).Where(a => a.Object.id == vozac.id).FirstOrDefault();
            await firebase.Child("Vozac").Child(brisi.Key).DeleteAsync();

        }
        public async Task<List<RecenzijeMod>> VratiOceneNegativne()
        {
            return (await firebase
              .Child("Ocene")
              .OnceAsync<RecenzijeMod>()).Where(a=>a.Object.zvezde < 3).Select(ocen => new RecenzijeMod()
              {
                  vozac = ocen.Object.vozac,
                  zvezde = ocen.Object.zvezde,
                  napomena = ocen.Object.napomena

              }).ToList();

        }


    }
}
