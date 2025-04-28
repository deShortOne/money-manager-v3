-- DROP tables
DROP TABLE IF EXISTS account_new;
DROP TABLE IF EXISTS account_user;

-- Create the new tables to replace the old one
CREATE TABLE public.account_new (
    id serial PRIMARY KEY,
    name character varying(50) NOT NULL
);

CREATE TABLE public.account_user (
    id serial PRIMARY KEY,
    users_id integer REFERENCES users,
    account_id integer REFERENCES account_new,
    user_owns_account boolean NOT NULL,
    UNIQUE (users_id, account_id)
);

-- insert data into new account table
INSERT INTO account_new ( name )
SELECT DISTINCT initcap(name)
FROM account;

-- create temporary table that translates ids from old account table to new
CREATE TABLE temp_OldAccountToNewAccountIds
AS  SELECT
        account.id old_id,
        account_new.id new_id,
        account.users_id users_id
    FROM account
    INNER JOIN account_new
        ON initcap(account.name) = initcap(account_new.name);

-- insert data into new account_user table
INSERT INTO public.account_user (users_id, account_id, user_owns_account)
SELECT users_id, new_id, true
FROM temp_OldAccountToNewAccountIds
WHERE users_id <> -1;

INSERT INTO public.account_user (users_id, account_id, user_owns_account)
SELECT users_id, new_id, false
FROM (SELECT distinct new_id
	    FROM temp_OldAccountToNewAccountIds
	   WHERE users_id = -1)
CROSS JOIN (SELECT id users_id
	          FROM users
	         WHERE users.id <> -1);

-- drop references to old account table
ALTER TABLE ONLY bill
    DROP CONSTRAINT fk_account,
    DROP CONSTRAINT fk_payee;
ALTER TABLE ONLY register
    DROP CONSTRAINT fk_account,
    DROP CONSTRAINT fk_payee;

-- update values that referenced old data
UPDATE public.bill
    SET payee = (SELECT account_user.id
                   FROM temp_OldAccountToNewAccountIds
             INNER JOIN account_user
                     ON temp_OldAccountToNewAccountIds.new_id = account_user.account_id
                    AND (
                        temp_OldAccountToNewAccountIds.users_id = account_user.users_id
                        OR temp_OldAccountToNewAccountIds.users_id = -1)
                  WHERE temp_OldAccountToNewAccountIds.old_id = bill.payee
                    AND account_user.users_id = (
                        SELECT users_id
                          FROM account
                         WHERE account.id = bill.account_id
                    )),
   account_id = (SELECT account_user.id
              FROM account_user
        INNER JOIN temp_OldAccountToNewAccountIds
                ON account_user.account_id = temp_OldAccountToNewAccountIds.new_id 
               AND account_user.users_id = temp_OldAccountToNewAccountIds.users_id
             WHERE temp_OldAccountToNewAccountIds.old_id = bill.account_id);
UPDATE public.register
    SET payee = (SELECT account_user.id
                   FROM temp_OldAccountToNewAccountIds
             INNER JOIN account_user
                     ON temp_OldAccountToNewAccountIds.new_id = account_user.account_id
                    AND (
                        temp_OldAccountToNewAccountIds.users_id = account_user.users_id
                        OR temp_OldAccountToNewAccountIds.users_id = -1)
                  WHERE old_id = payee
                    AND account_user.users_id = (
                        SELECT users_id
                          FROM account
                         WHERE id = register.account_id
                    )),
   account_id = (SELECT account_user.id
              FROM account_user
        INNER JOIN temp_OldAccountToNewAccountIds
                ON account_user.account_id = temp_OldAccountToNewAccountIds.new_id 
               AND account_user.users_id = temp_OldAccountToNewAccountIds.users_id
             WHERE temp_OldAccountToNewAccountIds.old_id = register.account_id);

-- add back in reference to account table
ALTER TABLE ONLY public.bill
	ADD	CONSTRAINT fk_account
			FOREIGN KEY (account_id)
				REFERENCES public.account_user(id),
	ADD	CONSTRAINT fk_payee
			FOREIGN KEY (payee)
				REFERENCES public.account_user(id);
ALTER TABLE ONLY public.register
	ADD	CONSTRAINT fk_account
			FOREIGN KEY (account_id)
				REFERENCES public.account_user(id),
	ADD	CONSTRAINT fk_payee
			FOREIGN KEY (payee)
				REFERENCES public.account_user(id);

-- drop temporary table that translated ids from old account table to new
DROP TABLE temp_OldAccountToNewAccountIds;

-- drop old account table
DROP TABLE public.account;

-- Rename the old table to be deleted
ALTER TABLE account_new RENAME TO account;

-- Rename tables altered in regards to accounts to form a ubiquitous language
ALTER TABLE public.bill
    RENAME account_id TO payer_user_id;
ALTER TABLE public.bill
    RENAME payee TO payee_user_id;
ALTER TABLE public.register
    RENAME account_id TO payer_user_id;
ALTER TABLE public.register
    RENAME payee TO payee_user_id;
ALTER TABLE public.bill
	RENAME CONSTRAINT fk_account TO fk_payer;
ALTER TABLE public.register
	RENAME CONSTRAINT fk_account TO fk_payer;
