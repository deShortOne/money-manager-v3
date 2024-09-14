INSERT INTO register (payee, amount, datePaid, category_id) VALUES
	('Company A', 1800, '2024-08-28', 1),
	('Phone company', 10, '2024-08-01', 2),
	('Landlord A', 500, '2024-08-01', 3),
	('Supermarket', 25, '2024-08-01', 4),
	('Supermarket', 23, '2024-08-08', 4),
	('Supermarket', 27, '2024-08-15', 4),
	('Hobby item', 150, '2024-08-09', 5);

INSERT INTO bill (payee, amount, nextduedate, frequency, categoryid, monthday) VALUES
    ('supermarket a', 23, '2024-09-03', 'Weekly', 4, 3),
    ('company a', 100, '2024-08-30', 'Monthly', 1, 30);

INSERT INTO BudgetCategory VALUES 
	(1, 1, 1800),
	(2, 2, 10),
	(2, 3, 500),
	(2, 4, 100),
	(4, 5, 50);
