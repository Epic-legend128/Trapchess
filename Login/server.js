const express = require('express');
const app = express()
const http = require('http');
const mongoose = require('mongoose');
const PORT = 8080;

var mongoURI = 'mongodb+srv://admin:Limestick69@log.l1hddbu.mongodb.net/Log';

mongoose.connect(mongoURI, { useNewUrlParser: true, useUnifiedTopology: true });

require('./model/user');
const account = mongoose.model('users');

// server = ws.listen(PORT)

app.listen(PORT, () => {
    console.log(`[STARTED] -> Server connected to port ${PORT}`);
});

app.get('/', async (req, res) => {
    console.log(`[CONNECTION] -> User Connection -> ` + user.username);
});

app.get('/account', async (req, res) => {
    const { rUsername, rEmail, rPassword } = req.query;
    if (rUsername == null || rPassword == null) {
        res.sendFile(__dirname + "/website/signup.html");
        return;
    }

    var user = await account.findOne({ username: rUsername });
    if (user == null) {
        console.log(`[ACCOUNT EXCEPTION] -> Creating new account...`);
        var entry = new account({
            username: rUsername,
            email: rEmail,
            password: rPassword,
            recentAuth: Date()
        });
        await entry.save();
        res.send(entry);
        console.log(`[ACCOUNT EXCEPTION] -> User ${entry}`);
        return;
    } else {
        if (rPassword == user.password) {
            user.recentAuth = Date();
            await user.save();
            console.log(`[CONNECTION] -> User Connection -> ` + user.username);
            res.send(user);
            return;
        }
    }
    res.sendFile(__dirname + "/website/userNotAuth.html");
    return;
});
