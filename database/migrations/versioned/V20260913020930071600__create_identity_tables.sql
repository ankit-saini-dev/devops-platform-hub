CREATE TABLE platform.roles
(
    id         UUID PRIMARY KEY,
    name       VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT ck_roles_name_not_blank CHECK (CHAR_LENGTH(BTRIM(name)) > 0),
    CONSTRAINT ck_roles_name_trimmed CHECK (name = BTRIM(name))
);

CREATE TABLE platform.users
(
    id              UUID PRIMARY KEY,
    name            VARCHAR(100) NOT NULL,
    username        VARCHAR(100) NOT NULL,
    email           VARCHAR(254) NOT NULL,
    password_hash   TEXT NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT ck_users_name_not_blank CHECK (CHAR_LENGTH(BTRIM(name)) > 0),
    CONSTRAINT ck_users_name_trimmed CHECK (name = BTRIM(name)),
    CONSTRAINT ck_users_username_not_blank CHECK (CHAR_LENGTH(BTRIM(username)) > 0),
    CONSTRAINT ck_users_username_trimmed CHECK (username = BTRIM(username)),
    CONSTRAINT ck_users_username_not_email CHECK (POSITION('@' IN username) = 0),
    CONSTRAINT ck_users_email_not_blank CHECK (CHAR_LENGTH(BTRIM(email)) > 0),
    CONSTRAINT ck_users_email_trimmed CHECK (email = BTRIM(email)),
    CONSTRAINT ck_users_email_has_at_sign CHECK (POSITION('@' IN email) > 1 AND POSITION('@' IN email) < CHAR_LENGTH(email)),
    CONSTRAINT ck_users_password_hash_not_blank CHECK (CHAR_LENGTH(password_hash) > 0)
);

CREATE UNIQUE INDEX ux_users_username_lower
    ON platform.users (LOWER(username));

CREATE UNIQUE INDEX ux_users_email_lower
    ON platform.users (LOWER(email));

CREATE TABLE platform.user_roles
(
    user_id         UUID NOT NULL,
    role_id         UUID NOT NULL,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_roles_user
        FOREIGN KEY (user_id)
        REFERENCES platform.users (id)
        ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role
        FOREIGN KEY (role_id)
        REFERENCES platform.roles (id)
        ON DELETE CASCADE
);

INSERT INTO platform.roles (id, name)
VALUES
    ('c088ce4f-baaa-4f1e-8e3e-62f88c5d576c', 'User'),
    ('7792d3a1-1258-4ac8-b065-2dd2eab6ba21', 'Administrator'),
    ('4e6c9cbf-c0ee-4a5c-9189-dd3810581ee5', 'Operator');
