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

INSERT INTO users (name, password) VALUES
    ('root', 'IfC1pbsUdKwcX68HPvPybQ==.bfXuHix96vvlXfGqLpY+/kRgBnCbXCU/Kqu2uIY8M60='),
    ('secondary root', 'lH0GmZnlH6TAwD+2wQx1UA==.C4UPD8P66L/A4AKv77WTsN6CSl6Wobgyy0psL3OkO+s=');
INSERT INTO users VALUES
	(-1, 'external company do not use', 'workshop.conversation.projection');

INSERT INTO account (name) VALUES
    ('Bank A'),
    ('Bank B'),
	('Supermarket'),
	('Hobby Item'),
	('Landlord A'),
	('Supermarket A'),
	('Phone Company'),
	('Company A'),
	('Vet'),
	('Football Kit');

INSERT INTO account_user (users_id, account_id, user_owns_account) VALUES
    (1, 1, true),
    (2, 1, true),
    (1, 2, true),
    (1, 3, false),
    (1, 4, false),
    (1, 5, false),
    (1, 6, false),
    (1, 7, false),
    (1, 8, false),
    (1, 9, false),
    (1, 10, false),
    (2, 3, false),
    (2, 4, false),
    (2, 5, false),
    (2, 6, false),
    (2, 7, false),
    (2, 8, false),
    (2, 9, false),
    (2, 10, false);

INSERT INTO register (payee_user_id, amount, datePaid, category_id, payer_user_id) VALUES
	(9, 1800, '2024-08-28', 1, 1),
	(8, 10, '2024-08-01', 2, 1),
	(6, 500, '2024-08-01', 3, 1),
	(4, 25, '2024-08-01', 4, 3),
	(4, 23, '2024-08-08', 4, 3),
	(4, 27, '2024-08-15', 4, 3),
	(5, 150, '2024-08-09', 5, 1),
	(17, 1500, '2024-08-28', 1, 2),
	(18, 75, '2024-08-29', 6, 2),
	(19, 100, '2024-08-30', 5, 2);

INSERT INTO bill (payee_user_id, amount, nextduedate, frequency, category_id, monthday, payer_user_id) VALUES
    (7, 23, '2024-09-03', 'Weekly', 4, 3, 1),
    (9, 100, '2024-08-30', 'Monthly', 1, 30, 3),
    (17, 100, '2024-08-30', 'Monthly', 1, 30, 2);

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
