package main

import (
	"fmt"
	"net/http"

	"github.com/alecngo/PetFinderService/handlers"
	"github.com/alecngo/PetFinderService/petfinderapi"
	"github.com/gorilla/mux"
	"github.com/joho/godotenv"
	"github.com/rs/cors"
)

func main() {
	// Load environment variables from .env file
	err := godotenv.Load("../config.env")
	if err != nil {
		fmt.Println("Error loading .env file", err)
		return
	}

	_, err = petfinderapi.GetClient()
	if err != nil {
		return
	}

	r := mux.NewRouter()

	// Set up routes
	r.HandleFunc("/nearby", handlers.FindPetsHandler).Methods("GET")
	r.HandleFunc("/findpet/{id}", handlers.GetAnimalByIdHandler).Methods("GET")

	// Start server
	handler := cors.Default().Handler(r)
	http.ListenAndServe(":8081", handler)
}
