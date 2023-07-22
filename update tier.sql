SELECT @high := (MAX(lutadores.Pontos) / 9)
FROM lutadores;
UPDATE lutadores
SET Tier = CASE
WHEN pontos > @high * 8 THEN 9
WHEN pontos > @high * 7 THEN 8
WHEN pontos > @high * 6 THEN 7
WHEN pontos > @high * 5 THEN 6
WHEN pontos > @high * 4 THEN 5
WHEN pontos > @high * 3 THEN 4
WHEN pontos > @high * 2 THEN 3
WHEN pontos > @high * 1 THEN 2
ELSE 1
END
