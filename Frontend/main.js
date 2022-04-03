import { Hotel } from "./hotel.js";
import { Location } from "./location.js";
import { Room } from "./room.js";
import { Season } from "./season.js";
import { User } from "./user.js";

var seasonDiv = document.getElementById("season")
var locationsDiv = document.getElementById("locations")
var searchBtn = document.getElementById("search-btn")
var hotelsDiv = document.getElementById("hotels")

var user = new User()
var loginForm = document.getElementById("login-form")

document.getElementById("login-btn").onclick = function() {
    var username = document.getElementById("username")
    var pass = document.getElementById("password")

    fetch("https://localhost:5001/User/GetUser/" + username.value + "/" + pass.value).then(p => {
        p.json().then(data => {
            console.log(data.status);
            if(data.status == 404)
                alert("Wrong password!")
            else{
                user = new User(data.userID, data.username, data.password, data.role)
                loginForm.innerHTML=''

                fetch("https://localhost:5001/Season/GetSeasons").then(p => {
                    p.json().then(data => {
                        data.forEach(el => {
                            var season = new Season(el.seasonID, el.name, el.locationIDs)
                            season.DrawSeason(seasonDiv, user)
                        });
                    })
                })
            }
        })
    })

var numOfSeasons = 0
fetch("https://localhost:5001/Season/GetSeasonNumber").then(p => {
    p.json().then(data => {
        numOfSeasons = data + 1
    })
})


if(user.id == "1"){
    var addSeasonBtn = document.createElement("button")
    addSeasonBtn.classList.add("add-season-btn")
    addSeasonBtn.innerHTML = "Add a season"

    var inputName
    
    addSeasonBtn.onclick = function() {
        var namePrompt = prompt("Season name:")
        if(namePrompt != null){
            inputName = namePrompt

            fetch("https://localhost:5001/Season/PostSeason", {
                method: 'POST',
                mode: 'cors',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    seasonID: numOfSeasons.toString(),
                    name: inputName
                })
            }).then(resp => {
                if (resp.status == 204) {
                    const season = new Season(numOfSeasons, inputName)
                    season.DrawSeason(seasonDiv, user)
                    alert("Season succesfuly added!")
                }
                else{
                    alert("Error")
                }
            });
        }
    }

    seasonDiv.appendChild(addSeasonBtn)
}

searchBtn.onclick = function() {

    while(hotelsDiv.firstChild){
        hotelsDiv.removeChild(hotelsDiv.firstChild)
    }

    var locations = [];
    var checkboxes = locationsDiv.querySelectorAll('input[type=checkbox]:checked')
    for(var i = 0; i < checkboxes.length; i++){
        locations.push(checkboxes[i].value)
    }

    fetch("https://localhost:5001/Hotel/GetHotelsByLocations", {
        method: 'PUT',
        mode: 'cors',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(locations)
    }).then(resp => {
        resp.json().then(data => {
            data.forEach(element => {
                var hotel = new Hotel(element.hotelID, element.name, element.phone, element.picture, element.commentIDs, element.ratingIDs, element.reservations, element.transports)
                hotel.DrawHotel(hotelsDiv, user)
            });
        })
    });
}
}
export function DrawHotelInput(host, hid){

    var createHotelDiv = document.createElement("div")

    var submitHotel = document.createElement("button")
    submitHotel.id = "submit-hotel"
    submitHotel.innerHTML = "Submit a hotel"

    var nameInput = document.createElement("input")
    nameInput.id = "hotel-name-input"
    nameInput.type = "text"
    nameInput.placeholder = "Hotel name"
    
    var pictureInput = document.createElement("input")
    pictureInput.id = "hotel-picture-input"
    pictureInput.type = "text"
    pictureInput.placeholder = "Picture name"

    var phoneInput = document.createElement("input")
    phoneInput.id = "hotel-phone-input"
    phoneInput.type = "text"
    phoneInput.placeholder = "Hotel phone"

    var roomsDiv = document.createElement("div")
    roomsDiv.id = "rooms-div"

    var addRoomBtn = document.createElement("button")
    addRoomBtn.id = "add-room-btn"
    addRoomBtn.innerHTML = "Add a room"
    roomsDiv.appendChild(addRoomBtn)

    if(hid != undefined){
        fetch("https://localhost:5001/Room/GetHotelsRooms/" + hid).then(p => {
            p.json().then(data => {
                data.forEach(el => {
                    var room = new Room(el.roomID, el.number, el.numOfBeds, el.hotelID)
                    room.DrawRoomInfo(roomsDiv, user)
                });
            })
        })
    }
    


    createHotelDiv.appendChild(nameInput)
    createHotelDiv.appendChild(pictureInput)
    createHotelDiv.appendChild(phoneInput)
    createHotelDiv.appendChild(submitHotel)

    createHotelDiv.appendChild(roomsDiv)

    host.appendChild(createHotelDiv)
}