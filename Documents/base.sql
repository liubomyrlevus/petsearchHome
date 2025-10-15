CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS registered_users (
    user_id SERIAL PRIMARY KEY,
    email TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    user_type TEXT CHECK (user_type IN ('individual', 'shelter')) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    last_login TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE,
    is_admin BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS sessions (
    session_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    session_token TEXT UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP,
    last_activity TIMESTAMP DEFAULT NOW(),
    is_valid BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS individuals (
    individual_id SERIAL PRIMARY KEY,
    user_id INT UNIQUE NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    phone TEXT,
    city TEXT,
    district TEXT,
    additional_info TEXT,
    photo_url TEXT
);

CREATE TABLE IF NOT EXISTS shelters (
    shelter_id SERIAL PRIMARY KEY,
    user_id INT UNIQUE NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    name TEXT UNIQUE NOT NULL,
    contact_person TEXT,
    phone TEXT,
    additional_phone TEXT,
    address TEXT,
    description TEXT,
    facebook_url TEXT,
    instagram_url TEXT,
    website_url TEXT,
    logo_url TEXT
);

CREATE TABLE IF NOT EXISTS listings (
    listing_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    animal_type TEXT CHECK (animal_type IN ('dog','cat','bird','other')) NOT NULL,
    breed TEXT,
    age_months INT,
    sex TEXT CHECK (sex IN ('male','female','unknown')),
    size TEXT CHECK (size IN ('small','medium','large')),
    color TEXT,
    city TEXT,
    district TEXT,
    description TEXT,
    special_needs TEXT,
    status TEXT CHECK (status IN ('draft','pending','active','rejected','archived')) DEFAULT 'draft',
    views_count INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    moderation_comment TEXT
);

CREATE TABLE IF NOT EXISTS photos (
    photo_id SERIAL PRIMARY KEY,
    listing_id INT NOT NULL REFERENCES listings(listing_id) ON DELETE CASCADE,
    url TEXT NOT NULL,
    is_primary BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS health_info (
    health_id SERIAL PRIMARY KEY,
    listing_id INT UNIQUE NOT NULL REFERENCES listings(listing_id) ON DELETE CASCADE,
    vaccinations TEXT,
    sterilized BOOLEAN,
    chronic_diseases TEXT,
    treatment_history TEXT
);

CREATE TABLE IF NOT EXISTS favorites (
    favorite_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    listing_id INT NOT NULL REFERENCES listings(listing_id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT NOW(),
    UNIQUE (user_id, listing_id)
);

CREATE TABLE IF NOT EXISTS conversations (
    conversation_id SERIAL PRIMARY KEY,
    user1_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    user2_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    listing_id INT REFERENCES listings(listing_id) ON DELETE SET NULL,
    last_message_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS messages (
    message_id SERIAL PRIMARY KEY,
    conversation_id INT NOT NULL REFERENCES conversations(conversation_id) ON DELETE CASCADE,
    sender_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    content TEXT NOT NULL,
    is_read BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT NOW()
);


CREATE TABLE IF NOT EXISTS reviews (
    review_id SERIAL PRIMARY KEY,
    reviewer_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    reviewed_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    is_moderated BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS reports (
    report_id SERIAL PRIMARY KEY,
    reporter_id INT NOT NULL REFERENCES registered_users(user_id) ON DELETE CASCADE,
    reported_type TEXT CHECK (reported_type IN ('listing','user','message')) NOT NULL,
    reported_id INT NOT NULL,
    reason TEXT,
    description TEXT,
    status TEXT CHECK (status IN ('pending','confirmed','rejected')) DEFAULT 'pending',
    created_at TIMESTAMP DEFAULT NOW(),
    resolved_at TIMESTAMP
);