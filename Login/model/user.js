const mongoose = require('mongoose');
const { Schema } = mongoose;
const account = new Schema({
    username: String,
    email: String,
    password: String,
    recentAuth: Date
});
mongoose.model('users', account);