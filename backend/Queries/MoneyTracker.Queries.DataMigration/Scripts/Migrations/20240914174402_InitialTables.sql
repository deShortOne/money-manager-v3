-- DROP tables
DROP TABLE IF EXISTS account;
DROP TABLE IF EXISTS bill;
DROP TABLE IF EXISTS budgetcategory;
DROP TABLE IF EXISTS budgetgroup;
DROP TABLE IF EXISTS category;
DROP TABLE IF EXISTS register;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS user_id_to_token;

-- CREATE tables
CREATE TABLE public.account (
    id serial PRIMARY KEY,
    name character varying(50) NOT NULL,
	users_id integer
);

CREATE TABLE public.bill (
    id serial PRIMARY KEY,
    payee integer NOT NULL,
    amount numeric NOT NULL,
    nextduedate date NOT NULL,
    frequency varchar(50) NOT NULL,
    category_id integer NOT NULL,
    monthday integer NOT NULL,
	account_id integer NOT NULL
);

CREATE TABLE public.budgetcategory (
	users_id integer NOT NULL,
    budget_group_id integer NOT NULL,
    category_id integer NOT NULL,
    planned numeric NOT NULL
);

CREATE TABLE public.budgetgroup (
    id serial PRIMARY KEY,
    name character varying(50) NOT NULL
);

CREATE TABLE public.category (
    id serial PRIMARY KEY,
    name character varying(50) NOT NULL
);

CREATE TABLE public.register (
    id serial PRIMARY KEY,
    payee character varying(50) NOT NULL,
    amount numeric NOT NULL,
    datepaid date NOT NULL,
    category_id integer NOT NULL,
	account_id integer NOT NULL
);

CREATE TABLE public.users (
    id serial PRIMARY KEY,
    name character varying(50) NOT NULL,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE public.user_id_to_token (
    user_id SERIAL,
    token TEXT,
    expires timestamp with time zone
);

-- Add constraints
ALTER TABLE ONLY public.account
    ADD CONSTRAINT fk_user
			FOREIGN KEY (users_id)
				REFERENCES public.users(id);
				
ALTER TABLE ONLY public.bill
    ADD CONSTRAINT fk_category
			FOREIGN KEY (category_id)
				REFERENCES public.category(id),
	ADD	CONSTRAINT fk_account
			FOREIGN KEY (account_id)
				REFERENCES public.account(id),
	ADD	CONSTRAINT fk_payee
			FOREIGN KEY (payee)
				REFERENCES public.account(id);

ALTER TABLE ONLY public.budgetcategory
    ADD CONSTRAINT fk_users
			FOREIGN KEY (users_id)
				REFERENCES public.users(id),
	ADD	CONSTRAINT fk_budget_group
			FOREIGN KEY (budget_group_id)
				REFERENCES public.budgetgroup(id),
	ADD	CONSTRAINT fk_category
			FOREIGN KEY (category_id)
				REFERENCES public.category(id),
    ADD CONSTRAINT user_budgetgroup_category 
   			UNIQUE (users_id, budget_group_id, category_id);
			
ALTER TABLE ONLY public.category
    ADD CONSTRAINT categoryuniquename UNIQUE (name);

ALTER TABLE ONLY public.register
    ADD CONSTRAINT fk_category
			FOREIGN KEY (category_id)
				REFERENCES public.category(id),
	ADD	CONSTRAINT fk_account
			FOREIGN KEY (account_id)
				REFERENCES public.account(id);

ALTER TABLE ONLY public.user_id_to_token
    ADD CONSTRAINT fk_user
        FOREIGN KEY(user_id) 
            REFERENCES users(id);
