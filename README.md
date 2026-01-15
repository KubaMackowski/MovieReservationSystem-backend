# Movie Reservation System

## Uruchomienie aplikacji

Do uruchomienia aplikacji wymagana jest aplikacja Docker

1. **Sklonuj repozytoria:**
    ```sh
    git clone https://github.com/KubaMackowski/MovieReservationSystem-frontend.git
    git clone https://github.com/KubaMackowski/MovieReservationSystem-backend.git
    ```
2. **Uruchom kontener docker w repozytorium backend, a następnie na repozytorium frontend:**
    ```sh
    docker-compose up -d --build
    ```
3. **Uruchom aplikację:**
   Aplikacja powinna działać pod adresem `http://localhost:3000`. Możesz to sprawdzić, otwierając przeglądarkę i wpisując ten adres.

Dodatkowe usługi to:
http://localhost:5050  -  PgAdmin (login: admin@domena.pl hasło: supersecretpass)

Hasło do bazy danych to: postgrespass

http://localhost:8080/swagger  -  Dokumentacja API
