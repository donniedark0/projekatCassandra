import { User } from "./user.js"
import { Comment } from "./comment.js"
import { Transport } from "./transport.js"
import { Room } from "./room.js"
import { DrawHotelInput } from "./main.js"

export class Hotel{
    constructor(id, name, pic, phone, comids, ratids, res, trids){
        this.id = id
        this.name = name
        this.picture = pic
        this.phone = phone
        this.commentIDs = comids
        this.ratingIDs = ratids
        this.reservations = res
        this.transportIDs = trids
    }

    DrawHotel(host, user){
        var thisptr = this
        
        var singleHotel = document.createElement("div")
        singleHotel.classList.add("single-hotel")
        singleHotel.value = thisptr.id
        
        var hotelName = document.createElement("h4")
        hotelName.innerHTML = this.name

        var hotelPic = document.createElement("img")
        hotelPic.classList.add("hotel-pic")
        hotelPic.src = 'pics/' + this.picture + '.jpg'

        /*var reservationInfo = document.createElement("div")
        reservationInfo.classList.add("hotel-desc")
        reservationInfo.innerHTML = this.description*/

        singleHotel.appendChild(hotelPic)
        singleHotel.appendChild(hotelName)
        host.appendChild(singleHotel)
        //singleHotel.appendChild(reservationInfo)
        var roomInfo = document.createElement("div")
        roomInfo.id = "room-info"
        hotelPic.onclick = function() {
            host.innerHTML = ''

            var leftSide = document.createElement("div")
            leftSide.classList.add("left-side")

            var rightSide = document.createElement("div")
            rightSide.classList.add("right-side")


            var bigSingleHotel = document.createElement("div")
            bigSingleHotel.classList.add("big-single-hotel")

            var hotelInfo = document.createElement("div")
            hotelInfo.classList.add("hotel-info-div")

            var picNameRating = document.createElement("div")
            picNameRating.classList.add("pic-name-rating")
            
            var pic = document.createElement("img")
            pic.classList.add("hotel-big-picture")
            pic.src = 'pics/' + thisptr.picture + ".jpg"

            picNameRating.appendChild(pic)

            var nameDiv = document.createElement("div")
            nameDiv.classList.add("hotel-name-and-btns")

            var name = document.createElement("h3")
            name.classList.add("hotel-big-name")
            name.innerHTML = thisptr.name

            nameDiv.appendChild(name)

            picNameRating.appendChild(nameDiv)

            var ratingDiv = document.createElement("div")
            ratingDiv.classList.add("hotel-big-rating")

            var rating = document.createElement("h3")
            rating.classList.add("hotel-rating")
            
            
            fetch("https://localhost:5001/Rating/ClaculateHotelRatings/" + thisptr.id,{
                method: 'PUT',
                mode: 'cors'
            }).then( p => {
                p.json().then(data => {
                    rating.innerHTML = "Rating: " + data
                });
            });

            ratingDiv.appendChild(rating)
            picNameRating.appendChild(ratingDiv)

            hotelInfo.appendChild(picNameRating)

            var commentSection = document.createElement("div")
            commentSection.classList.add("comment-section")

                var ratingBtns = document.createElement("div")
                for(var i=1; i<6; i++){
                    var mark = document.createElement("button")
                    mark.id = "mark-btn-" + i
                    mark.innerHTML = i
                    mark.value = i
                    ratingBtns.appendChild(mark)
                }

                var numOfRatings
               fetch("https://localhost:5001/Rating/GetRatingNumber").then(p => {
                    p.json().then(data => {
                        numOfRatings = data + 1
                    })
                })
                ratingBtns.querySelectorAll('button').forEach(element => {
                    element.onclick = function () {
                        fetch("https://localhost:5001/Rating/PostRating/" + user.id + "/" + thisptr.id, {
                            method: 'POST',
                            mode: 'cors',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({
                                ratingID: String(numOfRatings),
                                mark: String(this.value)
                            })
                        }).then(resp => {
                            if (resp.status == 204) {
                                fetch("https://localhost:5001/Rating/ClaculateHotelRatings/" + thisptr.id,{
                                    method: 'PUT',
                                    mode: 'cors'
                                }).then( p => {
                                    p.json().then(data => {
                                        rating.innerHTML = "Rating: " + data
                                        numOfRatings += 1
                                    });
                                })
                            }
                            else if(resp.status == 409){
                                fetch("https://localhost:5001/Rating/EditRating", {
                                    method: 'PUT',
                                    mode: 'cors',
                                    headers: {
                                        'Content-Type': 'application/json'
                                    },
                                    body: JSON.stringify({
                                        ratingID: numOfRatings,
                                        mark: Number(this.value)
                                    })
                                }).then( resp => {
                                        if (resp.status == 204) {
                                            fetch("https://localhost:5001/Rating/CalculateRating/" + thisptr.id,{
                                                method: 'PUT',
                                                mode: 'cors'
                                            }).then( p => {
                                                p.json().then(data => {
                                                    rating.innerHTML = "Rating: " + data
                                                });
                                            })
                                        }
                                    })
                            }
                            else{
                                alert("Error")
                            }
                        });
                    }
                })

                var newCommentDiv = document.createElement("div")
                newCommentDiv.classList.add("new-comment-div")

                var newCommentInput = document.createElement("textarea")
                newCommentInput.id = "comment-textarea"

                var addCommentBtn = document.createElement("button")
                addCommentBtn.classList.add("add-comment-btn")
                addCommentBtn.innerHTML = "Comment"

                newCommentDiv.appendChild(newCommentInput)
                newCommentDiv.appendChild(addCommentBtn)
                commentSection.appendChild(newCommentDiv)

                var numOfCom
                fetch("https://localhost:5001/Comment/GetCommentNumber").then(p => {
                    p.json().then(data => {
                        numOfCom = data + 1
                    })
                })
                addCommentBtn.onclick = function() {

                        fetch("https://localhost:5001/Comment/PostComment/" + user.id + "/" + thisptr.id, {
                            method: 'POST',
                            mode: 'cors',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({
                                commentid: numOfCom.toString(),
                                content: document.getElementById("comment-textarea").value,
                                userid: user.username
                            })
                        }).then(resp => {
                            if (resp.status == 204) {
                                const comment = new Comment(numOfCom, document.getElementById("comment-textarea").value, user.username)
                                comment.DrawComment(commentSection, user, thisptr.id)
                                alert("Comment succesfuly added!")
                                numOfCom += 1
                            }
                            else{
                                alert("Error")
                            }
                        })
                }
                ratingDiv.appendChild(ratingBtns)

            fetch("https://localhost:5001/Comment/GetHotelsComments/" + thisptr.id,{
                method: 'PUT',
                mode: 'cors'
            }).then( p => {
                p.json().then(data => {
                    data.forEach(el => {
                        var comment = new Comment(el.commentID, el.content, el.userID)
                        comment.DrawComment(commentSection, user, thisptr.id)
                    });
                })
            });

            leftSide.appendChild(hotelInfo)
            leftSide.appendChild(commentSection)

            bigSingleHotel.appendChild(leftSide)

            var reservationInfo = document.createElement("div")
            reservationInfo.classList.add("reservation-info")

            var datesDiv = document.createElement("div")
            datesDiv.classList.add("dates")

            var dateFrom = document.createElement("input")
            dateFrom.id = "date-from"
            dateFrom.type = "date"

            var dateTo = document.createElement("input")
            dateTo.id = "date-to"
            dateTo.type = "date"

            datesDiv.appendChild(dateFrom)
            datesDiv.appendChild(dateTo)

            var transportDiv = document.createElement("div")
            transportDiv.classList.add("transport")

            var textTr = document.createElement("h3")
            textTr.innerHTML="Transport:"

            fetch("https://localhost:5001/Transport/GetTransports").then( p => {
                p.json().then(data => {
                    data.forEach(el => {
                        const transport = new Transport(el.transportID, el.type)
                        transport.DrawTransport(transportDiv)
                    });
                });
            });

            transportDiv.appendChild(textTr)
    

            var bedsDiv = document.createElement("div")
            bedsDiv.classList.add("beds-div")

            var bedsList = document.createElement("select")
            bedsList.id = "beds-list"

            var lbl = document.createElement("label")
            lbl.innerHTML = "Number of beds: "
            
            for(var i = 1; i < 6; i++){
                var bedNum = document.createElement("option")
                bedNum.value = i
                bedNum.innerHTML = i
                bedsList.appendChild(bedNum)
            }
            bedsDiv.appendChild(lbl)
            bedsDiv.appendChild(bedsList)

          

            var roomSearch = document.createElement("button")
            roomSearch.classList.add("room-search")
            roomSearch.innerHTML = "Search rooms"

            reservationInfo.appendChild(datesDiv)
            reservationInfo.appendChild(transportDiv)
            reservationInfo.appendChild(bedsDiv)
            reservationInfo.appendChild(roomSearch)

            

            roomSearch.onclick = function () {
                roomInfo.innerHTML = ''
                var select = document.getElementById('beds-list')
                var beds = select.options[select.selectedIndex].value
                var from = document.getElementById("date-from").value
                var to = document.getElementById("date-to").value
                var transportRadios = transportDiv.querySelectorAll("input")
                var transport
                transportRadios.forEach(element => {
                    if(element.checked)
                    transport = element.value
                });
                console.log(user);
                fetch("https://localhost:5001/Room/GetAvailableHotelsRooms/" + thisptr.id + "?dateFrom=" + from + "&dateTo=" + to + "&numOfBeds=" + beds, {
                    method: 'PUT',
                    mode: 'cors'
                }).then( p => {
                    p.json().then(data => {
                        data.forEach(el => {
                            var room = new Room(el.roomID, el.number, el.numOfBeds, el.hotelID)
                            room.DrawRoom(roomInfo, user, transport, to, from)
                        });
                    })
                })
            }
            

            rightSide.appendChild(reservationInfo)
            rightSide.appendChild(roomInfo)

            bigSingleHotel.appendChild(rightSide)


            host.appendChild(bigSingleHotel)
        }

        if(user.id == "1"){
            var editHotel = document.createElement("button")
            editHotel.classList.add("edit-delete-hotel")
            editHotel.innerHTML = "Edit"

            singleHotel.appendChild(editHotel)

            editHotel.onclick = function () {
                var hotelDiv = document.getElementById("hotels")
                hotelDiv.innerHTML = ''

                DrawHotelInput(hotelDiv, thisptr.id)

                var numOfRooms = 0
                fetch("https://localhost:5001/Room/GetRoomNumber").then(p => {
                    p.json().then(data => {
                        numOfRooms = data + 1
                    })
                })
                var roomsdiv = document.getElementById("rooms-div")
                var roomNum
                var numberOfBeds
                var addRoomBttn = document.getElementById("add-room-btn")
                addRoomBttn.onclick = function() {
                    roomNum = prompt("Add a number of this room: ")
                    numberOfBeds = prompt("How many beds does this room have?")
            
                    fetch("https://localhost:5001/Room/PostRoom", {
                        method: 'POST',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            roomid: numOfRooms.toString(),
                            number: roomNum,
                            numOfBeds: numberOfBeds,
                            hotelid: thisptr.id
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            var newRoom = new Room(numOfRooms, roomNum, numberOfBeds, thisptr.id)
                            newRoom.DrawRoomInfo(roomsdiv, user)
                            numOfRooms += 1
                        }
                        else{
                            alert("Error")
                        }
                    });
            
                }

                document.getElementById("hotel-name-input").value = thisptr.name
                document.getElementById("hotel-picture-input").value = thisptr.picture
                document.getElementById("hotel-phone-input").value = thisptr.phone
  
                document.getElementById("submit-hotel").onclick = function(){

                    fetch("https://localhost:5001/Hotel/EditHotel", {
                        method: 'PUT',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            hotelid: thisptr.id,
                            name: document.getElementById("hotel-name-input").value,
                            picture: document.getElementById("hotel-picture-input").value,
                            phone: document.getElementById("hotel-phone-input").value
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            alert("Hotel succesfuly edited!")
                        }
                        else{
                            alert("Error")
                        }
                    });
                }

            }

            var deleteHotel = document.createElement("button")
            deleteHotel.classList.add("edit-delete-hotel")
            deleteHotel.innerHTML = "Delete"

            singleHotel.appendChild(deleteHotel)

            deleteHotel.onclick = function (e) {
                e.preventDefault();
                if (confirm("Do you want to delete this hotel?")) {
                    fetch("https://localhost:5001/Rating/DeleteHotelsRatings/" + thisptr.id, {
                        method: 'DELETE',
                        mode: 'cors'
                    })

                    fetch("https://localhost:5001/Comment/DeleteHotelsComments/" + thisptr.id, {
                        method: 'DELETE',
                        mode: 'cors'
                    })

                    fetch("https://localhost:5001/Reservation/DeleteHotelsReservations/" + thisptr.id, {
                        method: 'DELETE',
                        mode: 'cors'
                    })
                    fetch("https://localhost:5001/Hotel/DeleteHotel/" + thisptr.id, {
                        method: 'DELETE',
                        mode: 'cors'
                    }).then(resp => {
                        if (resp.status == 204) {
                            while(singleHotel.firstChild){
                                singleHotel.firstChild.remove()
                            }
                            singleHotel.remove();
                            alert("Hotel sucessfully deleted");
                        }
                    });
                }
            }
        }

    }
}