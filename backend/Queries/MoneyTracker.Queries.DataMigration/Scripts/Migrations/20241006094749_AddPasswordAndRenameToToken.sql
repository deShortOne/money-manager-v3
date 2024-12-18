ALTER TABLE user_id_to_guid
    RENAME TO user_id_to_token;

ALTER TABLE user_id_to_token
    RENAME COLUMN guid
        TO token;

ALTER TABLE users
    ADD password VARCHAR(255) DEFAULT 'pass';

ALTER TABLE users
    ALTER COLUMN password DROP DEFAULT,
    ALTER COLUMN password SET NOT NULL;
