export class Room{
    constructor(id, number, nob, hotelid){
        this.id = id
        this.number = number
        this.numOfBeds = nob
        this.hotelid = hotelid
    }

    DrawRoomInfo(host, user){

        var singleRoom = document.createElement("div")
        singleRoom.classList.add("single-room")
        
        var numLbl = document.createElement("div")
        numLbl.classList.add("num-lbl-room")
        numLbl.innerHTML = 'Room number: ' + this.number

        var bedNumLbl = document.createElement("div")
        bedNumLbl.classList.add("bed-lbl-room")
        bedNumLbl.innerHTML = "Bed number: " + this.numOfBeds

        singleRoom.appendChild(numLbl)
        singleRoom.appendChild(bedNumLbl)
        host.appendChild(singleRoom)

        if(user.id == "1"){
            var thisptr = this
            var deleteRoom = document.createElement("button")
            deleteRoom.classList.add("edit-delete");
            deleteRoom.innerHTML = "&times;"

            singleRoom.appendChild(deleteRoom)

            deleteRoom.onclick = function (e) {
                e.preventDefault();
                if (confirm("Do you want to delete this room?")) {
                    console.log("https://localhost:5001/Room/DeleteRoom/" + thisptr.id + "/" + thisptr.hotelid);
                    fetch("https://localhost:5001/Room/DeleteRoom/" + thisptr.id + "/" + thisptr.hotelid, {
                        method: 'DELETE',
                        mode: 'cors'
                    }).then(resp => {
                        if (resp.status == 204) {
                            while(singleRoom.firstChild){
                                singleRoom.firstChild.remove()
                            }
                            singleRoom.remove();
                            alert("Room sucessfully deleted");
                        }
                    });
                }
            }
        }
    }

    DrawRoom(host, user, tr, to, from){
        var thisptr = this

        var roomPlusRes = document.createElement("div")

        this.DrawRoomInfo(roomPlusRes, user)

        var resBtn = document.createElement("button")
        resBtn.classList.add("res-btn-room")
        resBtn.innerHTML = "Make a reservation"

        var numOfRes = 0
            fetch("https://localhost:5001/Reservation/GetReservationNumber").then(p => {
                p.json().then(data => {
                    numOfRes += data
                })
            })
            numOfRes+=1

        resBtn.onclick = function() {
            fetch("https://localhost:5001/Reservation/PostReservation", {
                method: 'POST',
                mode: 'cors',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    reservationID: String(numOfRes),
                    hotelID: thisptr.hotelid,
                    roomID: thisptr.id,
                    userID: user.id,
                    transportID: tr,
                    dateTo: to,
                    dateFrom: from
                })
            }).then(resp => {
                if (resp.status == 204) {
                    alert("Reservation was succesfuly made!")
                }
                else{
                    alert("Error")
                }
            });
        }
        

        roomPlusRes.appendChild(resBtn)
        host.appendChild(roomPlusRes)
    }
}