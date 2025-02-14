import {Link} from "react-router-dom";

export default function Home() {
    return (
        <div className="flex flex-col gap-6 p-28 px-3 max-w-6xl mx-auto min-h-screen">
            <h1 className='text-3xl font-bold lg:text-6xl'>Welcome to my Blog</h1>
            <p className='text-gray-500 text-xs sm:text-sm'>
                Here you&apos;ll find a variety of articles and tutorials on topics such as
                web development, software engineering, and programming languages.
            </p>
            <Link
                to='/search'
                className='text-xs sm:text-sm text-teal-500 font-bold hover:underline'
            >
                View all posts
            </Link>
        </div>
    );
}