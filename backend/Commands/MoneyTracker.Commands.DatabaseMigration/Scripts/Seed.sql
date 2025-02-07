INSERT INTO Category (name) VALUES
	-- Income
	('Wages & Salary : Net Pay'),
	-- Committed Expenses
	('Bills : Cell Phone'),
	('Bills : Rent'),
	('Groceries'),
	-- Irregular Expenses
	('Hobby'),
    ('Pet Care');

INSERT INTO BudgetGroup (name) VALUES 
	('Income'),
	('Committed Expenses'),
	('Fun'),
	('Irregular Expenses'),
	('Savings & Debt'),
	('Retirement');

INSERT INTO users(name, password) VALUES
    ('root', 'IfC1pbsUdKwcX68HPvPybQ==.bfXuHix96vvlXfGqLpY+/kRgBnCbXCU/Kqu2uIY8M60='),
    ('secondary root', 'lH0GmZnlH6TAwD+2wQx1UA==.C4UPD8P66L/A4AKv77WTsN6CSl6Wobgyy0psL3OkO+s=');
INSERT INTO users VALUES
	(-1, 'external company do not use', 'workshop.conversation.projection');

INSERT INTO account(name, users_id) VALUES
    ('bank a', 1),
    ('bank b', 1),
    ('bank a', 2),
	('Company A', -1),
	('Phone company', -1),
	('Landlord A', -1),
	('Supermarket', -1),
	('Hobby item', -1),
	('Vet', -1),
	('Football kit', -1),
	('supermarket a', -1),
	('company a', -1);

INSERT INTO register (payee, amount, datePaid, category_id, account_id) VALUES
	(4, 1800, '2024-08-28', 1, 1),
	(5, 10, '2024-08-01', 2, 1),
	(6, 500, '2024-08-01', 3, 1),
	(7, 25, '2024-08-01', 4, 2),
	(7, 23, '2024-08-08', 4, 2),
	(7, 27, '2024-08-15', 4, 2),
	(8, 150, '2024-08-09', 5, 1),
	(4, 1500, '2024-08-28', 1, 3),
	(9, 75, '2024-08-29', 6, 3),
	(10, 100, '2024-08-30', 5, 3);

INSERT INTO bill (payee, amount, nextduedate, frequency, category_id, monthday, account_id) VALUES
    (11, 23, '2024-09-03', 'Weekly', 4, 3, 1),
    (12, 100, '2024-08-30', 'Monthly', 1, 30, 2),
    (12, 100, '2024-08-30', 'Monthly', 1, 30, 3);

INSERT INTO BudgetCategory VALUES 
	(1, 1, 1, 1800),
	(1, 2, 2, 10),
	(1, 2, 3, 500),
	(1, 2, 4, 100),
	(1, 4, 5, 50);

INSERT INTO user_id_to_token VALUES
    (1, 'token 1', '2025-02-03 23:24:13.126961+00'),
    (1, 'token 2', '2025-02-04 23:24:13.126961+00'),
    (1, 'token 3', '2025-02-02 23:24:13.126961+00');
