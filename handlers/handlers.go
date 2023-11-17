package handlers

import (
	"encoding/json"
	"log"
	"net/http"

	"github.com/alecngo/PetFinderService/petfinderapi"
	"github.com/gorilla/mux"
)

// handleHTTPError simplifies error handling in HTTP handlers
func handleHTTPError(w http.ResponseWriter, message string, err error, statusCode int) {
	if err != nil {
		log.Println(err)
		http.Error(w, message, statusCode)
	}
}

// setCommonHeaders sets common headers for CORS and Content-Type
func setCommonHeaders(w http.ResponseWriter) {
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Content-Type", "application/json")
}

// FindPetsHandler for '/nearby' endpoint
func FindPetsHandler(w http.ResponseWriter, r *http.Request) {
	setCommonHeaders(w)
	if r.Method == "OPTIONS" {
		return
	}

	client, err := petfinderapi.GetClient()
	if err != nil {
		handleHTTPError(w, "Failed to initialize Petfinder client", err, http.StatusInternalServerError)
		return
	}

	zip := r.URL.Query().Get("zip")
	distance := r.URL.Query().Get("distance")

	myParams := petfinderapi.NewPetSearchParams()
	myParams.AddParam("location", zip)
	myParams.AddParam("distance", distance)
	myParams.AddParam("sort", "distance")

	myAnimals, err := client.GetAnimals(myParams)
	if err != nil {
		handleHTTPError(w, "Failed to retrieve animal details", err, http.StatusInternalServerError)
		return
	}

	json.NewEncoder(w).Encode(myAnimals.Animals)
}

// GetAnimalByIdHandler for '/findpet/{id}' endpoint
func GetAnimalByIdHandler(w http.ResponseWriter, r *http.Request) {
	setCommonHeaders(w)
	if r.Method == "OPTIONS" {
		return
	}

	vars := mux.Vars(r)
	id := vars["id"]

	client, err := petfinderapi.GetClient()
	if err != nil {
		handleHTTPError(w, "Failed to initialize Petfinder client", err, http.StatusInternalServerError)
		return
	}

	animal, err := client.GetAnimalById(id)
	if err != nil {
		handleHTTPError(w, "Failed to retrieve animal details", err, http.StatusInternalServerError)
		return
	}

	json.NewEncoder(w).Encode(animal)
}
