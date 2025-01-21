import {initializeApp} from "firebase/app";



const firebaseConfig = {
    apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
    authDomain: "blog-5217e.firebaseapp.com",
    projectId: "blog-5217e",
    storageBucket: "blog-5217e.firebasestorage.app",
    messagingSenderId: "422569611808",
    appId: "1:422569611808:web:284a83adc5aff15779a245"
};

// Initialize Firebase
export const app = initializeApp(firebaseConfig);

