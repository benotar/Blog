import {Link, useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";
import {Button, Spinner} from "flowbite-react";
import CommentSection from "../components/CommentSection.jsx";

const PostPage = () => {
    const {postSlug} = useParams();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(false);
    const [post, setPost] = useState(null);

    useEffect(() => {
        const fetchPost = async () => {
            try {
                const {data} = await $axios.get(`post/get-posts?searchTerm=${postSlug}`);

                if (!data.isSucceed) {
                    setError(true);
                    setLoading(false);
                    return;
                }

                setError(false);
                setLoading(false);
                setPost(data.payload.data.items[0]);
            } catch (error) {
                console.log(`Post page catch error: ${error.message}`);
                setError(true);
                setLoading(false);
            }
        }
        fetchPost();
    }, [postSlug]);

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-screen">
                <Spinner size="xl"/>
            </div>
        );
    }

    return (
        <main className="p-3 flex flex-col max-w-6xl mx-auto min-h-screen">
            <h1
                className="text-3xl mt-10 p-3 text-center font-serif max-w-2xl mx-auto lg:text-4xl"
            >
                {post && post.title}
            </h1>
            <Link
                className="self-center mt-5"
                to={`/search?category=${post && post.category}`}
            >
                <Button
                    color="gray"
                    pill
                    size="xs"
                >
                    {post && post.category}
                </Button>
            </Link>
            <img
                src={post && post.imageUrl}
                alt={post && post.title}
                className="mt-10 p-3 max-h-[600px] w-full object-cover"
            />
            <div className="flex justify-between p-3 border-b border-slate-500 mx-auto w-full max-w-2xl
            text-xs">
                <span className="italic">
                    {post && new Date(post.updatedAt).toLocaleDateString()}
                </span>
                <span>
                    {
                        post && (post.content.length / 1000).toFixed(0) > 1
                            ? `${(post.content.length / 1000).toFixed(0)} mins read`
                            : "1 min read"
                    }
                </span>
            </div>
            <div
                className="p-3 max-w-2xl mx-auto w-full post-content"
                dangerouslySetInnerHTML={{__html: post && post.content}}
            ></div>
            <CommentSection
                postId={post && post.id}
            />
        </main>
    );
};

export default PostPage;