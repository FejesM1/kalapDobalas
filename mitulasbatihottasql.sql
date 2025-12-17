-- Adatbázis: `szelesbalas_kalap`
-- A "mitulatbatihotta" nevet lecseréltük, hogy komolyabbnak tűnjön a pályázaton.

CREATE DATABASE IF NOT EXISTS `szelesbalas_kalap` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `szelesbalas_kalap`;

-- --------------------------------------------------------

--
-- Tábla szerkezet: `versenyzok`
--

CREATE TABLE `versenyzok` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nev` varchar(100) NOT NULL,
  `pont1` int(11) DEFAULT 0,
  `pont2` int(11) DEFAULT 0,
  `pont3` int(11) DEFAULT 0,
  -- Az idők kellenek a háttérszámításhoz (pontegyezőség esetén dönt)
  `ido1` double DEFAULT 99.99,
  `ido2` double DEFAULT 99.99,
  `ido3` double DEFAULT 99.99,
  `legjobbpont` int(11) DEFAULT 0,
  `pillhely` int(11) DEFAULT 999,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Mintaadatok a Szélesbálási lakosokkal
--

INSERT INTO `versenyzok` (`nev`, `pont1`, `pont2`, `pont3`, `ido1`, `ido2`, `ido3`, `legjobbpont`, `pillhely`) VALUES
('Hantás Dezső', 0, 4, 6, 10.00, 6.20, 4.50, 6, 7),
('Csapos Pityu', 5, 5, 5, 5.10, 5.05, 5.01, 5, 8),
('Kovács "Kalapos" János', 8, 9, 7, 2.10, 1.12, 3.40, 9, 1),
('Nagy "Hurkás" Béla', 5, 8, 8, 5.50, 2.20, 2.10, 8, 3),
('Szabó Gizella (Gyesen)', 7, 6, 2, 3.50, 4.10, 8.80, 7, 5),
('Tóth Géza (Jegyző)', 2, 3, 4, 8.20, 7.10, 6.50, 4, 9),
('Kiss Elemér', 1, 1, 3, 9.10, 9.05, 7.20, 3, 10),
('Varga Jenő', 0, 0, 2, 10.00, 10.00, 8.50, 2, 11),
('Polgármester Úr', 9, 2, 5, 1.50, 8.20, 5.10, 9, 2),
('Özv. Pártosné', 6, 8, 4, 4.20, 2.30, 6.10, 8, 4),
('Ifj. Lakatos Brendon', 4, 5, 7, 6.20, 5.10, 3.20, 7, 6);