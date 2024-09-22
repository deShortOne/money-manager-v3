CREATE TABLE user_id_to_guid (
    user_id SERIAL,
    guid UUID,
    expires timestamp with time zone,
    CONSTRAINT fk_user
        FOREIGN KEY(user_id) 
            REFERENCES users(id)
);
