import {Link, useNavigate} from "react-router-dom";
import {Alert, Button, Label, Spinner, TextInput} from "flowbite-react";
import {useState} from "react";
import $axios from "../axios/axios.js";
import OAuth from "../components/OAuth.jsx";


export default function SignUp() {
    const [formData, setFormData] = useState({});
    const [errorMessage, setErrorMessage] = useState(null);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({...formData, [e.target.id]: e.target.value.trim()});
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!formData.username || !formData.email || !formData.password) {
            return setErrorMessage("Please fill out all fields.");
        }
        try {
            setLoading(true);
            setErrorMessage(null);
            const {data} = await $axios.post("auth/sign-up", formData);
            if (data.isSucceed === false) {
                return setErrorMessage(data.errorCode);
            } else {
                navigate("/sign-in");
            }
        } catch (error) {
            let errorMessage = error.message;
            if(error.code === "ECONNABORTED") {
                errorMessage = "Request timeout!";
            }
            signInFailure(errorMessage);
        }
    }

    return (
        <div className="min-h-screen mt-20">
            <div className="flex p-3 max-w-3xl mx-auto flex-col md:flex-row md:items-center gap-5">
                {/* Left */}
                <div className="flex-1">
                    <Link to="/" className="font-bold dark:text-white text-4xl">
                <span
                    className="px-2 py-1 bg-gradient-to-r from-indigo-500
                    via-purple-500 to-pink-500 rounded-lg text-white"
                >
                    Benotar_&apos;s
                </span>
                        Blog
                    </Link>
                    <p className="text-sm mt-5">
                        You can sign up with your email and password or with Google.
                    </p>
                </div>

                {/* Right */}
                <div className="flex-1">
                    <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
                        <div>
                            <Label value="Your username"/>
                            <TextInput
                                type="text"
                                placeholder="Username"
                                id="username"
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <Label value="Your email"/>
                            <TextInput
                                type="email"
                                placeholder="example@email.com"
                                id="email"
                                onChange={handleChange}
                            />
                        </div>
                        <div>
                            <Label value="Your password"/>
                            <TextInput
                                type="password"
                                placeholder="Password"
                                id="password"
                                onChange={handleChange}
                            />
                        </div>
                        <Button gradientDuoTone="purpleToPink" type="submit" disabled={loading}>
                            {
                                loading ? (
                                    <>
                                        <Spinner size="sm"/>
                                        <span className="pl-3">Loading...</span>
                                    </>
                                ) : "Sign Up"
                            }
                        </Button>
                        <OAuth/>
                    </form>
                    <div className="flex gap-2 text-sm mt-5">
                        <span>Have an account?</span>
                        <Link to="/sign-in" className="text-blue-500">
                            Sign In
                        </Link>
                    </div>
                    {
                        errorMessage && (
                            <Alert className="mt-5" color="failure">
                                {errorMessage}
                            </Alert>
                        )
                    }
                </div>
            </div>
        </div>
    );
}