
CREATE TABLE public.receipt_analysis_state (
    id character varying(50) NOT NULL,
    users_id integer NOT NULL,
    filename character varying(50) NOT NULL,
    url character varying(150) NOT NULL,
    state integer NOT NULL,
    constraint fk_users_id
        foreign key (users_id)
        REFERENCES users (id)
);
