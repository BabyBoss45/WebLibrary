﻿CREATE TABLE Genres ( 
  Id INT PRIMARY KEY NOT NULL,
  Name VARCHAR(512) NOT NULL
);


CREATE TABLE Books (
  Id BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
  Name VARCHAR(512) NOT NULL,    
  CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
  Date DATETIME NULL,
  Summary VARCHAR(MAX) DEFAULT NULL
);

CREATE TABLE BooksGenres (
  BookId BIGINT NOT NULL,
  GenreId INT NOT NULL
);

CREATE INDEX BooksGenres_BookId	ON BooksGenres	
(BookId);
CREATE INDEX BooksGenres_GenreId ON BooksGenres	
(GenreId);

-- добавить в новый запрос для запросов поиска имя дата автор жанр 

ALTER TABLE BooksGenres
  ADD CONSTRAINT BooksGenres_Books_Id FOREIGN KEY (BookId) REFERENCES Books (Id)
  ON DELETE CASCADE;

ALTER TABLE BooksGenres
  ADD CONSTRAINT BooksGenres_Genres_Id FOREIGN KEY (GenreId) REFERENCES Genres (Id)
  ON DELETE CASCADE;