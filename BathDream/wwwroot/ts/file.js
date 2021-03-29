var el = this.document.getElementById("content");
var User = /** @class */ (function () {
    function User(_name, _age) {
        this.name = _name;
        this.age = _age;
    }
    return User;
}());
var tom = new User("Том", 29);
el.innerHTML = "Имя: " + tom.name + " возраст: " + tom.age;
//# sourceMappingURL=file.js.map