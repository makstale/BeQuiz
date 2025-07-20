BeQuiz project - Programowanie Zaawansowane

BeQuiz to aplikacja webowa do tworzenia, edytowania i rozwiązywania quizów.

Zawartość dokumentacji
- Instrukcja uruchomienia
- Opis funcjonalnosci
- Struktura projektu
- System użytkowników i kontrola dostępu
- Technologie użyte w projekcie

Instrukcja uruchomienia
- Sklonuj projekt
git clone https://github.com/makstale/BeQuiz.git
cd BeQuiz
dotnet ef database update
dotnet run
Otworz w przeglądarce localhost'a po porcie w którym projekt sie zahostował (np. https://localhost:5001)

Technologie
- ASP.NET 8.0
- Entity Framework Core
- Microsoft Identity

Model
- MVC (Model-View-Controller)

Funkcje
- Rejestracja i logowanie
- Tworzenie quizów z pytaniami jednokrotnego wyboru
- Punktacja dla każdego pytania
- Rozwiązywanie quizów
- Zapis wyników i statystyki
- Ochrona dostępu: użytkownik widzi tylko swoje dane
- Możliwość tworzenia quizów z:
tytułem
opisem
dowolną liczbą pytań jednokrotnego wyboru

Pytania zawierają:
- treść
- punktację
- zestaw odpowiedzi (minimum 2, domyślnie 4)
- oznaczenie odpowiedzi jako poprawnej

Rozwiązywanie quizów
- Każdy zalogowany użytkownik może wypełniać quizy stworzone przez innych
- Po wypełnieniu wynik jest zapisywany i wyświetlany
- Przechowywane dane: wybrane odpowiedzi, wynik punktowy, czas wypełnienia

Statystyki quizu (dostępne tylko dla autora)
- Liczba użytkowników, którzy wypełnili quiz
- Średni wynik
- Dla każdego pytania: procentowy wybór każdej odpowiedzi

Edycja i usuwanie
- Możliwość edycji quizów, pytań i odpowiedzi
- Usuwanie quizów i ich składników możliwe tylko dla właściciela
